using System;
using System.Collections;
using UnityEngine;

#pragma warning disable 649
namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }

    public class CarController : MonoBehaviour
    {
        [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedType m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        //bool to check if car is AI or for user - set to false as standard
        private bool isUser = false;
        private bool inGear;
        private bool stalled = false;
        private bool enguineOff = true;

        //clutch controller variables
        private int lastClutchPos;
        private bool gearChanged = false;
        private bool clutchDown = false;

        //bool to stop car rolling back on load
        private bool startedForFirstTime = false;

        //reversing variables
        bool inReverse = false;

        public AudioClip stallClip;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude * 2.23693629f; }}
        public float MaxSpeed{get { return m_Topspeed; }}
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        // Use this for initialization
        private void Start()
        {
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;
            
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
            m_Rigidbody.velocity = new Vector3(0, 0, 0);
        }

        //allows script to differentiate between AI cars and User cars
        public void setUserControlled()
        {
            isUser = true;
        }

        public float getCurrentSpeed()
        {
            return CurrentSpeed;
        }

        //accessor to be used in CarAudio to check if to play enguine noises
        public bool isTurnedOff()
        {
            return enguineOff;
        }

        private void GearChanging()
        {
            if (isUser)
            {
                if (LogitechGSDK.LogiUpdate())
                {
                    //get wheel access as per manual
                    LogitechGSDK.DIJOYSTATE2ENGINES rec;
                    rec = LogitechGSDK.LogiGetStateUnity(0);

                    lastClutchPos = rec.rglSlider[0];

                    //check gearstick for which gear car is in
                    if (rec.rgbButtons[12] == 128)
                    {
                        //stops you from moving forward when reversing too fast
                        if (inReverse && CurrentSpeed > 5)
                        {
                            stall();
                        }

                        if (m_GearNum != 1)
                        {
                            if (!clutchDown)
                            {
                                stall();
                            }
                            gearChanged = true;
                            m_GearNum = 1;
                        }
                        inReverse = false;
                        inGear = true;
                    }
                    else if (rec.rgbButtons[13] == 128)
                    {
                        //stops you from moving forward when reversing too fast
                        if (inReverse && CurrentSpeed > 5)
                        {
                            stall();
                        }

                        if (m_GearNum != 2)
                        {
                            if (!clutchDown)
                            {
                                stall();
                            }
                            gearChanged = true;
                            m_GearNum = 2;
                        }
                        inReverse = false;
                        inGear = true;
                    }
                    else if (rec.rgbButtons[14] == 128)
                    {
                        //stops you from moving forward when reversing too fast
                        if (inReverse && CurrentSpeed > 5)
                        {
                            stall();
                        }

                        if (m_GearNum != 3)
                        {
                            if (!clutchDown)
                            {
                                stall();
                            }
                            gearChanged = true;
                            m_GearNum = 3;
                        }
                        inReverse = false;
                        inGear = true;
                    }
                    else if (rec.rgbButtons[15] == 128)
                    {
                        //stops you from moving forward when reversing too fast
                        if (inReverse && CurrentSpeed > 5)
                        {
                            stall();
                        }

                        if (m_GearNum != 4)
                        {
                            if (!clutchDown)
                            {
                                stall();
                            }
                            gearChanged = true;
                            m_GearNum = 4;
                        }
                        inReverse = false;
                        inGear = true;
                    }
                    else if (rec.rgbButtons[16] == 128)
                    {
                        //stops you from moving forward when reversing too fast
                        if (inReverse && CurrentSpeed > 5)
                        {
                            stall();
                        }

                        if (m_GearNum != 5)
                        {
                            if (!clutchDown)
                            {
                                stall();
                            }
                            gearChanged = true;
                            m_GearNum = 5;
                        }
                        inReverse = false;
                        inGear = true;
                    }
                    //reverse gear
                    else if (rec.rgbButtons[17] == 128)
                    {
                        //stops you from moving backwards when going forward too fast
                        if (!inReverse && CurrentSpeed > 5)
                        {
                            stall();
                        }

                        if (m_GearNum != 6)
                        {
                            if (!clutchDown)
                            {
                                stall();
                            }
                            gearChanged = true;
                            m_GearNum = 6;
                        }
                        inReverse = true;
                        inGear = true;
                    }

                    else // else not in gear
                    {
                        //checks if user has moved out fo gear without pressing the clutch
                        if(inGear && !clutchDown)
                        {
                            stall();
                        }
                        inGear = false;
                    }
                    
                }
            }
            else // allows automatic shifting of AI cars
            {
                float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
                float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
                float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

                if (m_GearNum > 0 && f < downgearlimit)
                {
                    m_GearNum--;
                }

                if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
                {
                    m_GearNum++;
                }
            }
            
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor)*(1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }


        private void CalculateGearFactor()
        {
            float f = (1/(float) NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(CurrentSpeed/MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
        }

        private IEnumerator wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        //function to cause stalling
        private void stall()
        {
            //play stall noise -- ensures only plays once
            if (!stalled)
            {
                AudioSource.PlayClipAtPoint(stallClip, transform.position);
            }
            enguineOff = true;
            stalled = true;
            //relay msg to screen
        }


        private void CalculateRevs()
        {
            if (isUser) //function split in 2 for user car and AI
            {
                if (LogitechGSDK.LogiUpdate())
                {
                    LogitechGSDK.DIJOYSTATE2ENGINES rec;
                    rec = LogitechGSDK.LogiGetStateUnity(0);

                    //no revs in not turned on
                    if (enguineOff)
                    {
                        Revs = 0;
                    }

                    else if (inGear && !clutchDown)
                    {
                        switch (m_GearNum)
                        {
                            case 1:
                                smoothRevs(80); // converts speed to rpm as per website in notes and converts to between 0 and 1 for code
                                break;
                            case 2:
                                smoothRevs(120);
                                break;
                            case 3:
                                smoothRevs(175);
                                break;
                            case 4:
                                smoothRevs(235);
                                break;
                            case 5:
                                smoothRevs(370);
                                break;
                            case 6:
                                smoothRevs(80);
                                break;
                            default: // else not in gear, so rest
                                Revs = 0.06f;
                                break;
                        }//switch
                        
                        if (Revs < 0.06 && !clutchDown)
                        {
                            stall();
                        }

                        else if (clutchDown)
                        {
                            if (Revs < 0.06)
                            {
                                Revs = 0.06f;
                            }
                            else
                            {
                                smoothRevsNeutral((float)(rec.lY - 32767) / -65535);
                            }
                        }
                                             
                    }
                    else // else not in gear
                    {
                        //allows out of gear revving in conjunction to the accellorator being pressed
                        smoothRevsNeutral((float)(rec.lY - 32767) / -65535);
                        //if not pressing the gas then revs will be at resting rate
                        if (Revs < 0.06)
                        {
                            Revs = 0.06f;
                        }
                    }
                }
            }
            else //Code for AI revs
            {
                // calculate engine revs (for display / sound)
                // (this is done in retrospect - revs are not used in force/power calculations)
                CalculateGearFactor();
                var gearNumFactor = m_GearNum / (float)NoOfGears;
                var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
                var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
                Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
            }
            
        }

        private void smoothRevsNeutral(float gas)
        {
            float revDifference = Revs - gas;
            if (revDifference >= 0.01f)
            {
                Revs -= 0.01f;
            }
            else if (revDifference <= -0.01f)
            {
                Revs += 0.01f;
            }
            else
            {
                Revs = gas;
            }
        }

        private void smoothRevs(int gearDivider)
        {
            float targetRevs = CurrentSpeed / gearDivider;
            float revDifference = Revs - targetRevs;
            if (revDifference >= 0.0005f)
            {
                Revs -= 0.0005f;
            }
            else if (revDifference <= -0.0005f)
            {
                Revs += 0.0005f;
            }
            else
            {
                Revs = targetRevs;
            }

        }


        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            if (enguineOff) // if turned off check for it to be turned back on
            {
                if (LogitechGSDK.LogiUpdate())
                {
                    LogitechGSDK.DIJOYSTATE2ENGINES rec;
                    rec = LogitechGSDK.LogiGetStateUnity(0);
                    if (rec.rgbButtons[0] == 128)
                    {
                        startedForFirstTime = true;
                        enguineOff = false;
                        if (stalled)
                        {
                            stalled = false;
                        }
                    }
                    if (rec.rgbButtons[3] == 128)
                    {
                        UnityEngine.XR.InputTracking.Recenter();
                    }

                    AccelInput = accel = 0.0f;
                    BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
                    ApplyDrive(accel, footbrake);
                    SteerHelper();

                    if (!startedForFirstTime)
                    {
                        m_Rigidbody.velocity = new Vector3(0, 0, 0);
                    }
                    
                }
                Revs = 0.0f;
            }
            else
            {

                for (int i = 0; i < 4; i++)
                {
                    Quaternion quat;
                    Vector3 position;
                    m_WheelColliders[i].GetWorldPose(out position, out quat);
                    m_WheelMeshes[i].transform.position = position;
                    m_WheelMeshes[i].transform.rotation = quat;
                }

                if (LogitechGSDK.LogiUpdate())
                {
                    LogitechGSDK.DIJOYSTATE2ENGINES rec;
                    rec = LogitechGSDK.LogiGetStateUnity(0);

                    if (rec.rgbButtons[3] == 128)
                    {
                        UnityEngine.XR.InputTracking.Recenter();
                    }

                    int currentClutchPosition = rec.rglSlider[0];

                    if (currentClutchPosition < -2500)
                    {
                        clutchDown = true;
                    }
                    else
                    {
                        clutchDown = false;
                    }

                    if (gearChanged)
                    {
                        int liftLimit = 3500;
                        
                        if(currentClutchPosition < 2500 && currentClutchPosition > -2500)
                        {
                            inGear = true;
                            clutchDown = false;
                            if (Revs < 0.1)
                            {
                                stall();
                                gearChanged = false;
                            }
                        }
                        //changed to stop stalling when changing gear quickly in higher gears
                        else if ((m_GearNum == 1 || m_GearNum == 2) && currentClutchPosition >= 2500 && (currentClutchPosition - lastClutchPos > liftLimit))
                        {
                            stall();
                            gearChanged = false;
                        }
                        else if (currentClutchPosition >= 2500)
                        {
                            gearChanged = false;
                        }
                        else
                        {
                            lastClutchPos = currentClutchPosition;
                        }
                    }
                }

                //clamp input values
                steering = Mathf.Clamp(steering, -1, 1);

                //if to make sure that car can't accel when not in gear
                if (inGear && !clutchDown)
                {
                    AccelInput = accel = Mathf.Clamp(accel, 0, 1);
                }
                else
                {
                    AccelInput = accel = 0.0f;
                }

                BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
                handbrake = Mathf.Clamp(handbrake, 0, 1);

                //Set the steer on the front wheels.
                //Assuming that wheels 0 and 1 are the front wheels.
                m_SteerAngle = steering * m_MaximumSteerAngle;
                m_WheelColliders[0].steerAngle = m_SteerAngle;
                m_WheelColliders[1].steerAngle = m_SteerAngle;

                SteerHelper();
                ApplyDrive(accel, footbrake);
                CapSpeed();

                //Set the handbrake.
                //Assuming that wheels 2 and 3 are the rear wheels.
                if (handbrake > 0f)
                {
                    var hbTorque = handbrake * m_MaxHandbrakeTorque;
                    m_WheelColliders[2].brakeTorque = hbTorque;
                    m_WheelColliders[3].brakeTorque = hbTorque;
                }

                CalculateRevs();
                GearChanging();

                AddDownForce();
                CheckForWheelSpin();
                TractionControl();
            }
            
        }


        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/3.6f) * m_Rigidbody.velocity.normalized;
                    break;
            }
        }


        private void ApplyDrive(float accel, float footbrake)
        {

            float thrustTorque;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 4f);
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;

                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;

                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;

            }

            

            for (int i = 0; i < 4; i++)
            {
                //if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                if (Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque*footbrake;
                }
                else if (inReverse) // prev code: else if(footbrake > 0)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
                    //m_WheelColliders[i].motorTorque = -m_ReverseTorque*footbrake;
                    m_WheelColliders[i].motorTorque = -m_ReverseTorque * accel;
                }
                if (LogitechGSDK.LogiUpdate())
                {
                    //get wheel access as per manual
                    LogitechGSDK.DIJOYSTATE2ENGINES rec;
                    rec = LogitechGSDK.LogiGetStateUnity(0);

                    //if break isnt pressed, dont break
                    if (rec.lRz == 32767)
                    {
                        m_WheelColliders[i].brakeTorque = 0;
                    }
                }
                
            }

        }


        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up*m_Downforce*
                                                         m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                m_WheelColliders[i].GetGroundHit(out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
                {
                    m_WheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying())
                    {
                        m_WheelEffects[i].PlayAudio();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i].PlayingAudio)
                {
                    m_WheelEffects[i].StopAudio();
                }
                // end the trail generation
                m_WheelEffects[i].EndSkidTrail();
            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);

                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;

                case CarDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;

                case CarDriveType.FrontWheelDrive:
                    m_WheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }


        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_WheelEffects[i].PlayingAudio)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
