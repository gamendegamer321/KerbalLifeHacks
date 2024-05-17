using HarmonyLib;
using KSP.FX.Timeline;
using KSP.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace KerbalLifeHacks.Hacks.LaunchCountdown;

[Hack("Will show a countdown during the pre-launch sequence.")]
public class LaunchCountdown : BaseHack
{
    private const float SequenceTime = 13.65f;
    
    private static TextMeshProUGUI _textBox;
    private static PlayableDirector _director;
    private static bool _isActive;

    public override void OnInitialized()
    {
        Game.Messages.PersistentSubscribe<LaunchSequenceInitiatedMessage>(_ => { _isActive = false; });
        HarmonyInstance.PatchAll(typeof(LaunchCountdown));
    }

    public void Update()
    {
        if (_isActive)
        {
            _textBox.text = $"T-{SequenceTime - _director.time:00.00}";
        }
    }

    [HarmonyPatch(typeof(SequenceControllerComponent), nameof(SequenceControllerComponent.PlayTimeLine),
        [typeof(string)])]
    [HarmonyPostfix]
    public static void CountdownStarted(
        // ReSharper disable once InconsistentNaming
        PlayableDirector ___director,
        // ReSharper disable once InconsistentNaming
        string TimelineName)
    {
        if (TimelineName != "Prelaunch")
        {
            return;
        }

        // If we don't have it already, find the text component
        if (_textBox == null)
        {
            var obj = GameObject.Find("GameManager/Default Game Instance(Clone)/UI Manager(Clone)" +
                                      "/Scaled Main Canvas/FlightHudRoot(Clone)/group_gobutton(Clone)" +
                                      "/Widget_GoButton/GRP-GoButton/TXT-Go/");

            if (obj == null || !obj.TryGetComponent(out _textBox))
            {
                return;
            }
        }

        _director = ___director;
        _isActive = true;
    }
}