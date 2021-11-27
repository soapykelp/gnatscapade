using UnityEngine;

public class AudioMute : MonoBehaviour
{
    [SerializeField]
    private Sprite muteButtonImage;
    [SerializeField]
    private Sprite unmuteButtonImage;

    private UnityEngine.UI.Image buttonImage;

    void Start() {
        buttonImage = this.GetComponent<UnityEngine.UI.Image>();

        // Call mute if volume is 0 to change button sprite
        if (AudioListener.volume == 0) {
            Mute();
        }
    }

    private void Mute() {
        AudioListener.volume = 0;
        buttonImage.sprite = unmuteButtonImage;
    }

    private void Unmute() {
        AudioListener.volume = 1;
        buttonImage.sprite = muteButtonImage;
    }

    public void OnMuteButton() {
        if (AudioListener.volume > 0) {
            // Not muted, so mute
            Mute();
        } else {
            Unmute();
        }
    }
}
