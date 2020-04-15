using BarRaider.SdTools;
using Creative.Platform.CoreAudio.Externals;
using Creative.Platform.CoreAudio.Interfaces;
using Creative.Platform.Devices.Features;
using Creative.Platform.Devices.Features.SpeakerConfigs;
using Creative.Platform.Devices.Models;
using Creative.Platform.Devices.Models.CDCDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SDSoundBlasterX
{
    class Program
    {
        static void Main(string[] args)
        {
            // Uncomment this line of code to allow for debugging
            while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }

            // Test Bed
            //SpeakerMode.SpeakerModeExternal; //Possibly SPDIF out?
            //SpeakerMode.SpeakerModeHeadPhone; //Possible headset out?


            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(
           "SELECT * FROM Win32_SoundDevice");

            ManagementObjectCollection objCollection = objSearcher.Get();

            ManagementObject soundblaster = null;

            foreach (ManagementObject obj in objCollection)
            {
                foreach (PropertyData property in obj.Properties)
                {
                    Console.Out.WriteLine(String.Format("{0}:{1}", property.Name, property.Value));

                    if (property.Name.Equals("Name") && property.Value.ToString().Contains("Sound Blaster"))
                    {
                        soundblaster = obj;
                    }
                }

                
            }

            Creative.Platform.Devices.Models.DeviceManager dm = DeviceManager.Instance;
            //.Collections.Concurrent.ConcurrentDictionary<string, DeviceEndpoint> des = dm.MixerDiscoveredDeviceEndpoints;
            //DeviceEndpoint mixerDevice = dm.GetMixerDeviceEndpoints(new Device {d });

            //  Define an IMM Device
            Creative.Platform.CoreAudio.Interfaces.IMMDeviceEnumerator imde = (Creative.Platform.CoreAudio.Interfaces.IMMDeviceEnumerator)new Creative.Platform.CoreAudio.Classes.MMDeviceEnumerator();

            IMMDevice imd = null;
            foreach (DeviceEndpoint de in dm.MixerDiscoveredDeviceEndpoints.Values)
            {
                int device_id = imde.GetDevice(de.DeviceEndpointId, out imd);
                if (imd != null)
                {
                    break;
                }
            }


            //  Generate a DeviceEndpoint
            DefaultDeviceEndpointFactory ddef = new Creative.Platform.Devices.Models.DefaultDeviceEndpointFactory();
            DeviceEndpoint dep = ddef.CreateDeviceEndPoint(imd);

            List<IDeviceRepository> dev_reps = dep.GetDeviceRepositories();

            //  Create the SoundCore
            //Creative.Platform.Devices.Features.SoundCore.SoundCoreRepository scr = 
            Creative.Platform.Devices.Features.Apo.ApoInfoWrapper aiw = new Creative.Platform.Devices.Features.Apo.ApoInfoWrapper(dep);
            Creative.Platform.Devices.Features.Apo.ApoFeatureChecker apc = new Creative.Platform.Devices.Features.Apo.ApoFeatureChecker(aiw);
            Creative.Platform.Devices.Features.Apo.PropStoreRepository psr = new Creative.Platform.Devices.Features.Apo.PropStoreRepository(dep.DeviceEndpointId, apc);

            IPropertyStore properties;
            imd.OpenPropertyStore(0u, out properties);
            CDCDeviceRepositoryInitializer cdcri = new CDCDeviceRepositoryInitializer();
            cdcri.Initialize(dep, properties);

            psr.GetSpeakerConfig();

            //Creative.Platform.Devices.Features.SpeakerConfigs.SpeakerConfigService scs = new SpeakerConfigService(dep);
            //Creative.Platform.Devices.Features.SpeakerConfigs.SoundCoreSpeakerConfigFeature scsrf = new SoundCoreSpeakerConfigFeature(dep,,, scs);

            //Creative.Platform.Devices.Features.SoundCore.SoundCoreRepository scr = new Creative.Platform.Devices.Features.SoundCore.SoundCoreRepository()

            try
            {
                
                psr.SetSpeakerConfig((int)SpeakerMode.SpeakerModeHeadPhone);

            }
            catch (Exception e)
            {

            }
            //scr.SetSpeakerConfig((int)SpeakerMode.SpeakerModeHeadPhone);

            //SDWrapper.Run(args);
        }
    }
}
