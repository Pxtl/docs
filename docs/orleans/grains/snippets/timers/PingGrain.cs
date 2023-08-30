﻿using Orleans.Runtime;
using Orleans.Timers;

namespace Timers;

public sealed class PingGrain : IGrainBase, IPingGrain, IDisposable
{
    private const string ReminderName = "ExampleReminder";

    private readonly IReminderRegistry _reminderRegistry;

    private IGrainReminder? _reminder;

    public  IGrainContext GrainContext { get; }

    public PingGrain(
        ITimerRegistry timerRegistry,
        IReminderRegistry reminderRegistry,
        IGrainContext grainContext)
    {
        // Register timer
        timerRegistry.RegisterTimer(
            grainContext,
            asyncCallback: static async state =>
            {
                // Omitted for brevity...
                // Use state

                await Task.CompletedTask;
            },
            state: this,
            dueTime: TimeSpan.FromSeconds(3),
            period: TimeSpan.FromSeconds(10));

        _reminderRegistry = reminderRegistry;

        GrainContext = grainContext;
    }

    public async Task Ping()
    {
        _reminder = await _reminderRegistry.RegisterOrUpdateReminder(
            callingGrainId: GrainContext.GrainId,
            reminderName: ReminderName,
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromHours(1));
    }

    void IDisposable.Dispose()
    {
        if (_reminder is not null)
        {
            _reminderRegistry.UnregisterReminder(
                GrainContext.GrainId, _reminder);
        }
    }
}
