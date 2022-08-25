using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Device.Gpio;
using System.Threading;
using System.Net.WebSockets;
using Iot.Device.RotaryEncoder;

namespace Client
{
    public class Program
    {
        private int pin = 10;
        private ClientWebSocket client = new ClientWebSocket();

        private void SendClientRequestOn(int cum, PinValueChangedEventArgs e) {
            // toggle the state of the LED every time the button is pressed
            if (e.ChangeType == PinEventTypes.Rising)
            {
                return;
            }
        }

        private void SendClientRequestOff(int cum, PinValueChangedEventArgs e) {
            // toggle the state of the LED every time the button is pressed
            if (e.ChangeType == PinEventTypes.Falling)
            {
                return;
            }
        }

        // https://docs.microsoft.com/en-us/samples/microsoft/windows-iotcore-samples/push-button/
        // ^ Good reference
        public static void Main(string[] args)
        {
            GpioDriver driver = new GpioDriver();
            using (var controller = new GpioController()) {
                controller.OpenPin(pin, PinMode.Output);

                // Check if input pull-up resistors are supported
                if (driver.IsPinModeSupported(pin, GpioPinDriveMode.InputPullUp))
                    driver.SetPinMode(pin, GpioPinDriveMode.InputPullUp);
                else
                    driver.SetPinMode(pin, GpioPinDriveMode.Input);
                // Set a debounce timeout to filter out switch bounce noise from a button press
                var Debounce = TimeSpan.FromMilliseconds(50);

                // Register for the ValueChanged event so our buttonPin_ValueChanged 
                // function is called when the button is pressed
                ValueChanged += SendClientRequestOn;
                
            }
        }
    }
}
