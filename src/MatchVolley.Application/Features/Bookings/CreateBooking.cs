using MediatR;
using MatchVolley.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MatchVolley.Application.Features.Bookings;

public sealed record CreateBookingCommand(
    Guid CourtId,
    DateTime StartUtc,
    TimeSpan Duration,
    string UserId) : IRequest<Guid>;

public sealed class CreateBookingHandler(IAppDbContext db) : IRequestHandler<CreateBookingCommand, Guid>
{
    public async Task<Guid> Handle(CreateBookingCommand req, CancellationToken ct)
    {
        var end = req.StartUtc + req.Duration;

        var availabilityOk = await db.CourtAvailabilities.AnyAsync(a =>
                a.CourtId == req.CourtId &&
                a.StartUtc <= req.StartUtc &&
                a.EndUtc >= end &&
                !a.IsBlocked, ct);

        if (!availabilityOk)
            throw new InvalidOperationException("Horário indisponível.");

        var conflict = await db.Bookings.AnyAsync(b =>
            b.CourtId == req.CourtId &&
            ((req.StartUtc >= b.StartUtc && req.StartUtc < b.EndUtc) ||
             (end > b.StartUtc && end <= b.EndUtc) ||
             (req.StartUtc <= b.StartUtc && end >= b.EndUtc)), ct);

        if (conflict)
            throw new InvalidOperationException("Conflito de reserva.");

        var price = await db.CourtAvailabilities
            .Where(a => a.CourtId == req.CourtId && a.StartUtc <= req.StartUtc && a.EndUtc >= end)
            .Select(a => a.Price)
            .FirstAsync(ct);

        // Nota: TenantId é aplicado por filtro global no SaveChanges.
        var booking = new Domain.Entities.Booking
        {
            CourtId = req.CourtId,
            UserId = req.UserId,
            StartUtc = req.StartUtc,
            EndUtc = end,
            Amount = price,
            Status = "Confirmed"
        };

        db.Bookings.Add(booking);
        await db.SaveChangesAsync(ct);
        return booking.Id;
    }
}
