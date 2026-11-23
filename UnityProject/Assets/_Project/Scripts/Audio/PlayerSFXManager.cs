using HorrorProject.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>Sound effects triggered by the player (pick-ups, locks, lantern, heartbeat...).</summary>
public class PlayerSFXManager : AudioChannel<PlayerSFXManager>
{
    [Header("Clips")]
    [SerializeField, FormerlySerializedAs("sfx_OpenDrawer")] private AudioClip openDrawer;
    [SerializeField, FormerlySerializedAs("sfx_CloseDrawer")] private AudioClip closeDrawer;
    [SerializeField, FormerlySerializedAs("sfx_lockpickOpened")] private AudioClip lockpickOpened;
    [SerializeField, FormerlySerializedAs("sfx_lockpickOpening")] private AudioClip lockpickOpening;
    [SerializeField, FormerlySerializedAs("sfx_woodStep")] private AudioClip woodStep;
    [SerializeField, FormerlySerializedAs("sfx_dontPick")] private AudioClip dontPick;
    [SerializeField, FormerlySerializedAs("sfx_pickLockpick")] private AudioClip pickLockpick;
    [SerializeField, FormerlySerializedAs("sfx_saveInInventory")] private AudioClip saveInInventory;
    [SerializeField, FormerlySerializedAs("sfx_pickKey")] private AudioClip pickKey;
    [SerializeField, FormerlySerializedAs("sfx_pickBattery")] private AudioClip pickBattery;
    [SerializeField, FormerlySerializedAs("sfx_pickNote")] private AudioClip pickNote;
    [SerializeField, FormerlySerializedAs("sfx_switch")] private AudioClip switchClick;
    [SerializeField, FormerlySerializedAs("sfx_reloadLantern")] private AudioClip reloadLantern;
    [SerializeField, FormerlySerializedAs("sfx_switchLanternOn")] private AudioClip switchLanternOn;
    [SerializeField, FormerlySerializedAs("sfx_openBag")] private AudioClip openBag;
    [SerializeField, FormerlySerializedAs("sfx_heartBeat1")] private AudioClip heartBeat1;
    [SerializeField, FormerlySerializedAs("sfx_heartBeat2")] private AudioClip heartBeat2;
    [SerializeField, FormerlySerializedAs("sfx_locked")] private AudioClip locked;
    [SerializeField, FormerlySerializedAs("sfx_chains")] private AudioClip chains;

    protected override void RegisterClips(SoundBank<AudioClip> clips)
    {
        clips.Add("OpenDrawer", openDrawer);
        clips.Add("CloseDrawer", closeDrawer);
        clips.Add("lockpickOpened", lockpickOpened);
        clips.Add("lockpickOpening", lockpickOpening);
        clips.Add("woodStep", woodStep);
        clips.Add("dontPick", dontPick);
        clips.Add("pickLockpick", pickLockpick);
        clips.Add("saveInInventory", saveInInventory);
        clips.Add("pickKey", pickKey);
        clips.Add("pickBattery", pickBattery);
        clips.Add("pickNote", pickNote);
        clips.Add("switch", switchClick);
        clips.Add("reloadLantern", reloadLantern);
        clips.Add("switchLanternOn", switchLanternOn);
        clips.Add("openBag", openBag);
        clips.Add("heartBeat1", heartBeat1);
        clips.Add("heartBeat2", heartBeat2);
        clips.Add("locked", locked);
        clips.Add("chains", chains);
    }

    public AudioSource PlaySFX(string soundName) => Play(soundName);
}
