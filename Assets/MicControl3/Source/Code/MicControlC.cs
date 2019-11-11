
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

#if UNITY_EDITOR
using System.IO;
#endif


namespace AS
{
    namespace MicControl
    {

        [RequireComponent(typeof(AudioSource))]

        public class MicControlC : MonoBehaviour
        {


            //should this controller be in loudness or spectrum mode (simple or advanced). This is also used by the editor script to show the correct visuals.
            public bool enableSpectrumData = false;


            //if false the below will override and set the mic selected in the editor
            public bool useDefaultMic = false;

            public bool ShowDeviceName = true;
            public bool SetDeviceSlot = false;

            public int InputDevice;
            string selectedDevice;


            public bool advanced = false;
            public bool spectrumDropdown = false;

            public AudioSource audioSource;
            //The maximum amount of sample data that gets loaded in, best is to leave it on 256, unless you know what you are doing. A higher number gives more accuracy but 
            //lowers performance allot, it is best to leave it at 256.
            public int amountSamples = 256;


            //the variables that do the users magic
            public float loudness;
            public float rawInput;
            public float[] spectrumData;
            public AnimationCurve spectrumCurve;

            //settings
            public float sensitivity = 500.0f;
            public Vector2 minMaxSensitivity = new Vector2(0.0f, 500.0f);

            public int bufferTime = 1;

            public enum freqList { _44100HzCD, _48000HzDVD }
            public freqList freq;
            public int setFrequency = 44100;


            public bool Mute = true;
            public bool debug = false;

            bool recording = true;


            public bool focused = true;
            public bool Initialized = false;


            public bool doNotDestroyOnLoad = false;



            void Start()
            {
                StartCoroutine(yieldedStart());

            }

            private IEnumerator yieldedStart()
            {


                //wait till level is loaded
                if (LoadingLevel())
                {

                    yield return 0;

                }

                //make this controller persistent
                if (doNotDestroyOnLoad)
                {
                    DontDestroyOnLoad(transform.gameObject);
                }

                yield return new WaitForSeconds(1);


                //return and throw error if no device is connected
                if (Microphone.devices.Length == 0)
                {
                    Debug.LogError("No connected device detected! Connect at least one device.");
                    Debug.LogError("No usable device detected! Try setting your device as the system's default.");
                    //return;
                }

                Initialized = false;
                if (!Initialized)
                {
                    InitMic();
                    //Initialized=true;
                }




            }









            //apply the mic input data stream to a float and or array;
            void Update()
            {

                //pause everything when not focused on the app and then re-initialize.


                if (focused && !LoadingLevel())
                {
                    if (!Initialized)
                    {
                        InitMic();
                        if (debug)
                        {
                            Debug.Log("mic started: " + selectedDevice);
                        }
                        //	Initialized=true;

                    }
                }


                if (!focused && Initialized)
                {
                    StopMicrophone();
                    if (debug)
                    {
                        Debug.Log("mic stopped");
                    }
                    return;
                }


                if (!Application.isPlaying && Initialized)
                {
                    //stop the microphone if you are clicking inside the editor and the player is stopped
                    StopMicrophone();
                    if (debug)
                    {
                        Debug.Log("mic stopped");
                    }
                    return;
                }

                if (LoadingLevel() && Initialized)
                {
                    //stop the microphone if you are clicking inside the editor and the player is stopped
                    StopMicrophone();
                    if (debug)
                    {
                        Debug.Log("mic stopped");
                    }
                    return;
                }


                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (focused && Initialized)
                {
                    //----------//----------//----------//----------//----------//----------//----------//----------//----------//----------
                    if (Microphone.IsRecording(selectedDevice) && recording)
                    {

                        //the simple strength to float method, used by most users. Outputs the mic's loudness into a single float
                        loudness = GetDataStream() * sensitivity;
                        rawInput = GetDataStream();

                        //----------//----------//----------//----------//----------//----------//----------//----------//----------//----------//----------
                        //the more advanced spectrum data analyses, for the advanced users. Outputs array of frequencies received from the microphone.

                        if (enableSpectrumData)
                        {
                            spectrumData = GetSpectrumAnalysis();


                            //----------//----------//----------//----------//----------//----------//----------//----------//----------//----------//----------
                            if (spectrumCurve.keys.Length <= spectrumData.Length)
                            {
                                //create a curvefield of none exists
                                for (int t = 0; t < spectrumData.Length; t++)
                                {

                                    spectrumCurve.AddKey(1 / spectrumData.Length + t, spectrumData[t]);


                                }

                            }
                            //represent the spectrum data in a curvefield
                            for (int t = 0; t < spectrumData.Length; t++)
                            {

                                spectrumCurve.MoveKey(1 / spectrumData.Length + t, new Keyframe(1 / spectrumData.Length + t, spectrumData[t]));//update keyframe value


                            }

                        }
                        //----------//----------//----------//----------//----------//----------//----------//----------//----------//----------//----------

                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    //Make sure the AudioSource volume is always 1
                    audioSource.volume = 1;


                }

            }







            public float GetDataStream()
            {

                if (Microphone.IsRecording(selectedDevice))
                {

                    float[] dataStream = new float[amountSamples];
                    float audioValue = 0f;
                    audioSource.GetOutputData(dataStream, 0);

                    //add up all the outputdata
                    for (int a = 0; a <= dataStream.Length - 1; a++)
                    {
                        audioValue = Mathf.Abs(audioValue + dataStream[a]);
                    }

                    //return the combined output data deviced by the sample amount to get the average loudness.
                    return audioValue / amountSamples;
                }
                return 0;

            }




            public float[] GetSpectrumAnalysis()
            {

                float[] dataSpectrum = new float[amountSamples];
                audioSource.GetSpectrumData(dataSpectrum, 0, FFTWindow.Rectangular);

                for (int i = 0; i <= dataSpectrum.Length - 1; i++)
                {

                    dataSpectrum[i] = Mathf.Abs(dataSpectrum[i] * sensitivity);

                }

                return dataSpectrum;

            }



            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Initialize microphone
            public void InitMic()
            {


                //select audio source
                if (!audioSource)
                {
                    audioSource = transform.GetComponent<AudioSource>();
                }

                //only Initialize microphone if a device is detected
                if (Microphone.devices.Length > 0)
                {

                    int i = 0;
                    //count amount of devices connected
                    foreach (string device in Microphone.devices)
                    {
                        i++;
                    }

                    //set selected device from isnpector as device number. (to find the device).
                    if (i >= 1 && !useDefaultMic)
                    {
                        selectedDevice = Microphone.devices[InputDevice];
                    }

                    //set the default device if enabled
                    if (useDefaultMic)
                    {
                        selectedDevice = Microphone.devices[0];
                    }




                    //Now that we know which device to listen to, lets set the frequency we want to record at

                    if (freq == freqList._44100HzCD)
                    {
                        setFrequency = 44100;

                    }

                    if (freq == freqList._48000HzDVD)
                    {
                        setFrequency = 48000;

                    }



                    //detect the selected microphone one time once in order to geth the first buffer.
                    audioSource.clip = Microphone.Start(selectedDevice, true, bufferTime, setFrequency);


                    //loop the playing of the recording so it will be realtime
                    audioSource.loop = true;
                    //if you only need the data stream values  check Mute, if you want to hear yourself ingame don't check Mute. 
                    //audioSource.mute = Mute;	



                    AudioMixer mixer = Resources.Load("MicControl3Mixer") as AudioMixer;
                    if (Mute)
                    {
                        mixer.SetFloat("MicControl3Volume", -80);
                    }
                    else
                    {
                        mixer.SetFloat("MicControl3Volume", 0);
                    }





                    //don't do anything until the microphone started up
                    while (!(Microphone.GetPosition(selectedDevice) > 0))
                    {
                        if (debug)
                        {
                            Debug.Log("Awaiting connection");
                        }
                    }
                    if (debug)
                    {
                        Debug.Log("Connected");
                    }

                    //Now that the basic initialisation is done, we are ready to start the microphone and gather data.
                    StartCoroutine(StartMicrophone());
                    recording = true;

                }

                Initialized = true;

            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 



            //for the above control the mic start or stop


            public IEnumerator StartMicrophone()
            {

                GetMicCaps();

                audioSource.clip = Microphone.Start(selectedDevice, true, bufferTime, setFrequency);//Starts recording

                while (!(Microphone.GetPosition(selectedDevice) > 0))
                {// Wait if a device is detected and  only then start the recording
                    if (debug)
                    {
                        Debug.Log("Waiting for connection:" + Time.deltaTime);
                    }
                    yield return 0;
                }
                if (debug)
                {
                    Debug.Log("started" + ", Freq (Hz): " + setFrequency + ", samples: " + amountSamples + ", sensitivity: " + sensitivity);
                }

                audioSource.Play(); // Play the audio source! 

                if (debug)
                {
                    Debug.Log("Receiving data");
                }
            }





            public void StopMicrophone()
            {
                audioSource.Stop();//Stops the audio
                Microphone.End(selectedDevice);//Stops the recording of the device  
                Initialized = false;
                recording = false;

            }



            void GetMicCaps()
            {
                int minFreq;
                int maxFreq;

                Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
                                                                                   //if the selected device has no frequency or is not sending data, the script wills top and throw out an error.
                if ((0 + maxFreq) == 0)
                    Debug.LogError("No frequency detected on device: " + selectedDevice + "... frequency= " + maxFreq);
                return;

            }



#if !UNITY_WEBGL
#if !UNITY_IOS
#if !UNITY_ANDROID

            //start or stop the script from running when the state is paused or not.
            void OnApplicationFocus(bool focus)
            {
                if (!Application.runInBackground)
                {
                    focused = focus;
                }
                else if (Application.isEditor)
                {
                    focused = focus;
                }

            }

            void OnApplicationPause(bool focus)
            {
                if (!Application.runInBackground)
                {
                    focused = !focus;
                }
                else if (Application.isEditor)
                {
                    focused = !focus;
                }
            }

            void OnApplicationExit(bool focus)
            {
                if (!Application.runInBackground)
                {
                    focused = focus;
                }
                else if (Application.isEditor)
                {
                    focused = focus;
                }
            }

#endif
#endif
#endif


            public bool LoadingLevel()
            {

                if (SceneManager.GetActiveScene().isLoaded)
                {
                    return false;
                }
                return true;
            }











#if UNITY_EDITOR

            //draw the gizmo
            void OnDrawGizmos()
            {

                Gizmos.DrawIcon(transform.position, "MicControlGizmo.tif", true);



                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //if gizmo folder does not exist create it
                if (!Directory.Exists(Application.dataPath + "/Gizmos"))
                {
                    Directory.CreateDirectory(Application.dataPath + "/Gizmos");
                }

                if (!File.Exists(Application.dataPath + "/Gizmos/MicControlGizmo.tif"))
                {
                    File.Copy(Application.dataPath + "/MicControl3/Resources/MicControlGizmo.tif", Application.dataPath + "/Gizmos/MicControlGizmo.tif");
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

            }

#endif















        }

    }
}