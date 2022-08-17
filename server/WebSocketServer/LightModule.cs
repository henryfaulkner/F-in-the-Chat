using System;
using System.Device.Gpio;
using System.Threading;

class LightModule {
    public void BlinkLights(int pin) {
        Console.WriteLine("Turning on light.");

        try {
            using (var controller = new GpioController()) {
                controller.OpenPin(pin, PinMode.Output);
                controller.Write(pin, PinValue.High);
                Thread.Sleep(1000);
                controller.Write(pin, PinValue.Low);
            }
        } catch(Exception exception) {
            Console.WriteLine($"exception: {exception}");
        }
    }
}