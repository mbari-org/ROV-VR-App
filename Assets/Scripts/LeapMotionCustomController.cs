using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;




/// <summary>
/// This script creates a custom controller for the leapmotion hands where 
/// the hand direction will serve as the pointer direction and the grip 
/// strength will serve as the click.
/// </summary>
public class LeapMotionCustomController : MonoBehaviour {
    // Although we could get hands through the API, the API does not include
    // convenient ways of getting Unity positions, so we will use these
    // to get the pose
    [SerializeField]
    public Leap.Unity.RiggedHand LHand;
    [SerializeField]
    public Leap.Unity.RiggedHand RHand;
    [SerializeField]
    public float clickThreshold = 0.5f;

    Controller controller;

    private Vector3 handPosition;
    private Quaternion handDirection;

    void Start()
    {
        controller = new Controller();
        //CurvedUIInputModule.ControlMethod = CurvedUIInputModule.CUIControlMethod.STEAMVR_2;
    }

    void Update()
    {
        Frame frame = controller.Frame();

        // Check to make sure hands are actually there
        if (frame.Hands.Count > 0)
        {
            List<Hand> hands = frame.Hands;
            Hand hand = hands[0]; // Use first hand

            if (hand.IsLeft)
            {
                // Get unity coordinates of the palm
                handPosition = LHand.GetPalmPosition();
                // Get unity rotation of the palm
                handDirection = LHand.GetPalmRotation();

            } else
            {
                // Get unity coordinates of the palm
                handPosition = RHand.GetPalmPosition();
                // Get unity rotation of the palm
                handDirection = RHand.GetPalmRotation();
            }

            // Adjust angle of pointer
            handDirection *= Quaternion.Euler(130f, 0, 0);

            // Set raycast start position
            // Offset raycast start slightly (so it doesn't hit our hand)            
            Vector3 offset = handDirection * new Vector3(0, 0, 0.2f);
            gameObject.transform.position = handPosition + offset;
            gameObject.transform.rotation = handDirection;

            // Get pinch strength of hand to function as click
            bool click = hand.PinchStrength > clickThreshold;

            // Update CurvedUIInputModule
            if (CurvedUIInputModule.ControlMethod == CurvedUIInputModule.CUIControlMethod.CUSTOM_RAY)
            {
                CurvedUIInputModule.CustomControllerRay = new Ray(gameObject.transform.position, gameObject.transform.forward);
                CurvedUIInputModule.CustomControllerButtonState = click;

                CurvedUIInputModule.Instance.PointerTransformOverride = gameObject.transform;
            } else
            {
                CurvedUIInputModule.Instance.PointerTransformOverride = null;
            }
        }
    }
}