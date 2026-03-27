using UnityEngine;

public class InputUpdater : MonoBehaviour
{
    private Vector2 smooth;

    void Update()
    {
        //Debug.Log(HandTrackingReceiver.Position);
        //Debug.Log(HandTrackingReceiver.Gesture);
        Vector2 mp = HandTrackingReceiver.Position;

        // инверсия Y (MediaPipe → Unity)
        //mp.y = 1f - mp.y;

        // при необходимости раскомментируй:
        mp.x = 1f - mp.x;

        Vector2 screenPos = new Vector2(
            mp.x * Screen.width,
            mp.y * Screen.height
        );

        // сглаживание
        smooth = Vector2.Lerp(smooth, screenPos, 0.2f);

        CustomInput.Position = smooth;

        CustomInput.SliceActive =
            HandTrackingReceiver.Gesture == "one_finger";
    }
}