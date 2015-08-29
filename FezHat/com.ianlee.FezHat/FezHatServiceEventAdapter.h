//-----------------------------------------------------------------------------
// <auto-generated> 
//   This code was generated by a tool. 
// 
//   Changes to this file may cause incorrect behavior and will be lost if  
//   the code is regenerated.
//
//   Tool: AllJoynCodeGenerator.exe
//
//   This tool is located in the Windows 10 SDK and the Windows 10 AllJoyn 
//   Visual Studio Extension in the Visual Studio Gallery.  
//
//   The generated code should be packaged in a Windows 10 C++/CX Runtime  
//   Component which can be consumed in any UWP-supported language using 
//   APIs that are available in Windows.Devices.AllJoyn.
//
//   Using AllJoynCodeGenerator - Invoke the following command with a valid 
//   Introspection XML file and a writable output directory:
//     AllJoynCodeGenerator -i <INPUT XML FILE> -o <OUTPUT DIRECTORY>
// </auto-generated>
//-----------------------------------------------------------------------------
#pragma once

namespace com { namespace ianlee { namespace FezHat {

// This class, and the associated EventArgs classes, exist for the benefit of JavaScript developers who
// do not have the ability to implement IFezHatService. Instead, FezHatServiceEventAdapter
// provides the Interface implementation and exposes a set of compatible events to the developer.
public ref class FezHatServiceEventAdapter sealed : [Windows::Foundation::Metadata::Default] IFezHatService
{
public:
    // Method Invocation Events
    event Windows::Foundation::TypedEventHandler<FezHatServiceEventAdapter^, FezHatSetRedLedStateCalledEventArgs^>^ SetRedLedStateCalled;
    event Windows::Foundation::TypedEventHandler<FezHatServiceEventAdapter^, FezHatGetRedLedStateCalledEventArgs^>^ GetRedLedStateCalled;
    event Windows::Foundation::TypedEventHandler<FezHatServiceEventAdapter^, FezHatSetRgbLedD2ColorCalledEventArgs^>^ SetRgbLedD2ColorCalled;
    event Windows::Foundation::TypedEventHandler<FezHatServiceEventAdapter^, FezHatSetRgbLedD3ColorCalledEventArgs^>^ SetRgbLedD3ColorCalled;
    event Windows::Foundation::TypedEventHandler<FezHatServiceEventAdapter^, FezHatGetLightSensorValueCalledEventArgs^>^ GetLightSensorValueCalled;
    event Windows::Foundation::TypedEventHandler<FezHatServiceEventAdapter^, FezHatGetTemperatureSensorValueCalledEventArgs^>^ GetTemperatureSensorValueCalled;

    // Property Read Events
    
    // Property Write Events

    // IFezHatService Implementation
    virtual Windows::Foundation::IAsyncOperation<FezHatSetRedLedStateResult^>^ SetRedLedStateAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info, _In_ bool interfaceMemberOn);
    virtual Windows::Foundation::IAsyncOperation<FezHatGetRedLedStateResult^>^ GetRedLedStateAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);
    virtual Windows::Foundation::IAsyncOperation<FezHatSetRgbLedD2ColorResult^>^ SetRgbLedD2ColorAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info, _In_ bool interfaceMemberOn, _In_ byte interfaceMemberRed, _In_ byte interfaceMemberGreen, _In_ byte interfaceMemberBlue);
    virtual Windows::Foundation::IAsyncOperation<FezHatSetRgbLedD3ColorResult^>^ SetRgbLedD3ColorAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info, _In_ bool interfaceMemberOn, _In_ byte interfaceMemberRed, _In_ byte interfaceMemberGreen, _In_ byte interfaceMemberBlue);
    virtual Windows::Foundation::IAsyncOperation<FezHatGetLightSensorValueResult^>^ GetLightSensorValueAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);
    virtual Windows::Foundation::IAsyncOperation<FezHatGetTemperatureSensorValueResult^>^ GetTemperatureSensorValueAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);


};

} } } 
