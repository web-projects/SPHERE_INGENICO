using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Management;

namespace IPA.DAL.RBADAL.Services
{
    public static class SerialPortService
    {
        
        private static string[] _serialPorts;
        private static ManagementEventWatcher pluggedIn;
        private static ManagementEventWatcher unplugged;

        public static event EventHandler<PortsChangedArgs> PortsChanged;

        public static void CleanUp()
        {
            pluggedIn.Stop();
            unplugged.Stop();
        }

        static SerialPortService()
        {
            _serialPorts = GetAvailablePorts();
            MonitorDeviceChanges();
        }

        private static void MonitorDeviceChanges()
        {
            try
            {
                var pluggedInQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
                var unpluggedQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");

                pluggedIn = new ManagementEventWatcher(pluggedInQuery);
                unplugged = new ManagementEventWatcher(unpluggedQuery);

                pluggedIn.EventArrived += (o, args) => RaisePortsChangedIfNecessary(EventType.Insertion);
                unplugged.EventArrived += (sender, eventArgs) => RaisePortsChangedIfNecessary(EventType.Removal);

                pluggedIn.Start();
                unplugged.Start();
            }
            catch (ManagementException ex)
            {
                //log item here
                throw (ex);
            }
        }

        private static void RaisePortsChangedIfNecessary(EventType eventType)
        {
            try
            {
                lock (_serialPorts)
                {
                    var availableSerialPorts = GetAvailablePorts();
                    switch (eventType)
                    {
                        case (EventType.Insertion):
                            {
                                var added = availableSerialPorts.Except(_serialPorts).ToArray();
                                _serialPorts = availableSerialPorts;
                                PortsChanged.Raise(null, new PortsChangedArgs(eventType, added));
                                break;
                            }
                        case (EventType.Removal):
                            {
                                var removed = _serialPorts.Except(availableSerialPorts).ToArray();
                                _serialPorts = availableSerialPorts;
                                PortsChanged.Raise(null, new PortsChangedArgs(eventType, removed));
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) where T : EventArgs
        {
            EventHandler<T> copy = handler;
            if (copy != null)
            {
                copy(sender, args);
            }
        }
    }

    public enum EventType
    {
        Insertion,
        Removal,
    }
    public class PortsChangedArgs : EventArgs
    {
        private readonly EventType _eventType;
        private readonly string[] _serialPorts;

        public PortsChangedArgs(EventType eventType, string[] serialPorts)
        {
            _eventType = eventType;
            _serialPorts = serialPorts;
        }
        public string[] SerialPorts
        {
            get { return _serialPorts; }
        }
        public EventType EventType
        {
            get { return _eventType; }
        }
    }
}
