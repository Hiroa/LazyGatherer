using System;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace LazyGatherer.Models;

public class AlertSoundConfig
{
    public readonly bool IsAnAlarm;
    public readonly uint SoundEffectId = 1;
    public readonly AlarmSoundEffect AlarmSoundEffect;

    public AlertSoundConfig(uint soundEffectId)
    {
        SoundEffectId = soundEffectId;
    }

    public AlertSoundConfig(AlarmSoundEffect alarmSoundEffect)
    {
        IsAnAlarm = true;
        AlarmSoundEffect = alarmSoundEffect;
    }

    public override bool Equals(object? obj)
    {
        return obj is AlertSoundConfig other &&
               IsAnAlarm == other.IsAnAlarm &&
               SoundEffectId == other.SoundEffectId &&
               AlarmSoundEffect == other.AlarmSoundEffect;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsAnAlarm, SoundEffectId, AlarmSoundEffect);
    }

    public static AlertSoundConfig FromConfig()
    {
        return Service.Config.GpAlertSoundIsAlarm
                   ? new AlertSoundConfig(Service.Config.GpAlertAlarmSoundEffect)
                   : new AlertSoundConfig(Service.Config.GpAlertSoundEffectId);
    }
}
