#region license

/*
DirectShowLib - Provide access to DirectShow interfaces via .NET
Copyright (C) 2007
http://sourceforge.net/projects/directshownet/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

#endregion

using System;
using System.Runtime.InteropServices;

namespace DirectShowLib
{
    #region COM Class Objects

    /// <summary>
    /// CLSID_StreamBufferRecordingAttributes
    /// </summary>
    [ComImport, Guid("CCAA63AC-1057-4778-AE92-1206AB9ACEE6")]
    public class StreamBufferRecordingAttributes
    {
    }

    /// <summary>
    /// CLSID_AudioRecord
    /// </summary>
    [ComImport, Guid("e30629d2-27e5-11ce-875d-00608cb78066")]
    public class AudioRecord
    {
    }

    /// <summary>
    /// CLSID_AVICo
    /// </summary>
    [ComImport, Guid("D76E2820-1563-11cf-AC98-00AA004C0FA9")]
    public class AVICo
    {
    }

    /// <summary>
    /// CLSID_AVIDoc
    /// </summary>
    [ComImport, Guid("D3588AB0-0781-11ce-B03A-0020AF0BA770")]
    public class AVIDoc
    {
    }

    /// <summary>
    /// CLSID_AviReader
    /// </summary>
    [ComImport, Guid("1b544c21-fd0b-11ce-8c63-00aa0044b51e")]
    public class AviReader
    {
    }

    /// <summary>
    /// CLSID_FGControl 
    /// </summary>
    [ComImport, Guid("e436ebb4-524f-11ce-9f53-0020af0ba770")]
    public class FGControl
    {
    }

  
    /// <summary>
    /// CLSID_FileSource
    /// </summary>
    [ComImport, Guid("701722e0-8ae3-11ce-a85c-00aa002feab5")]
    public class FileSource
    {
    }

    /// <summary>
    /// CLSID_FilterMapper
    /// </summary>
    [ComImport, Guid("e436ebb2-524f-11ce-9f53-0020af0ba770")]
    public class FilterMapper
    {
    }

    /// <summary>
    /// CLSID_ProtoFilterGraph
    /// </summary>
    [ComImport, Guid("e436ebb0-524f-11ce-9f53-0020af0ba770")]
    public class ProtoFilterGraph
    {
    }

    /// <summary>
    /// CLSID_MOVReader
    /// </summary>
    [ComImport, Guid("44584800-F8EE-11ce-B2D4-00DD01101B85")]
    public class MOVReader
    {
    }

    /// <summary>
    /// CLSID_VPObject
    /// </summary>
    [ComImport, Guid("CE292861-FC88-11d0-9E69-00C04FD7C15B")]
    public class VPObject
    {
    }

    /// <summary>
    /// CLSID_VPVBIObject
    /// </summary>
    [ComImport, Guid("814B9801-1C88-11d1-BAD9-00609744111A")]
    public class VPVBIObject
    {
    }

    /// <summary>
    /// CLSID_MPEG1Doc
    /// </summary>
    [ComImport, Guid("e4bbd160-4269-11ce-838d-00aa0055595a")]
    public class MPEG1Doc
    {
    }

    /// <summary>
    /// CLSID_TextRender
    /// </summary>
    [ComImport, Guid("e30629d3-27e5-11ce-875d-00608cb78066")]
    public class TextRender
    {
    }

    /// <summary>
    /// CLSID_CDeviceMoniker
    /// </summary>
    [ComImport, Guid("4315D437-5B8C-11d0-BD3B-00A0C911CE86")]
    public class CDeviceMoniker
    {
    }

    /// <summary>
    /// CLSID_DTFilter
    /// </summary>
    [ComImport, Guid("C4C4C4F2-0049-4E2B-98FB-9537F6CE516D")]
    public class DTFilter
    {
    }

    /// <summary>
    /// CLSID_ETFilter
    /// </summary>
    [ComImport, Guid("C4C4C4F1-0049-4E2B-98FB-9537F6CE516D")]
    public class ETFilter
    {
    }

    /// <summary>
    /// CLSID_FilterGraphPrivateThread
    /// </summary>
    [ComImport, Guid("a3ecbc41-581a-4476-b693-a63340462d8b")]
    public class FilterGraphPrivateThread
    {
    }


    /// <summary>
    /// CLSID_DtvCcFilter
    /// </summary>
    [ComImport, Guid("FB056BA0-2502-45B9-8E86-2B40DE84AD29")]
    public class DtvCcFilter
    {
    }


    /// <summary>
    /// CLSID_MSTVCaptionFilter
    /// </summary>
    [ComImport, Guid("2F7EE4B6-6FF5-4EB4-B24A-2BFC41117171")]
    public class MSTVCaptionFilter
    {
    }


    /// <summary>
    /// CLSID_SystemDeviceEnum
    /// </summary>
    [ComImport, Guid("62BE5D10-60EB-11d0-BD3B-00A0C911CE86")]
    public class CreateDevEnum
    {
    }


    /// <summary>
    /// CLSID_FilterGraph
    /// </summary>
    [ComImport, Guid("e436ebb3-524f-11ce-9f53-0020af0ba770")]
    public class FilterGraph
    {
    }


    /// <summary>
    /// CLSID_FilterGraphNoThread
    /// </summary>
    [ComImport, Guid("e436ebb8-524f-11ce-9f53-0020af0ba770")]
    public class FilterGraphNoThread
    {
    }


    /// <summary>
    /// CLSID_CaptureGraphBuilder2
    /// </summary>
    [ComImport, Guid("BF87B6E1-8C27-11d0-B3F0-00AA003761C5")]
    public class CaptureGraphBuilder2
    {
    }


    /// <summary>
    /// CLSID_DvdGraphBuilder
    /// </summary>
    [ComImport, Guid("FCC152B7-F372-11d0-8E00-00C04FD7C08B")]
    public class DvdGraphBuilder
    {
    }


    /// <summary>
    /// CLSID_CaptureGraphBuilder
    /// </summary>
    [ComImport, Guid("BF87B6E0-8C27-11d0-B3F0-00AA003761C5")]
    public class CaptureGraphBuilder
    {
    }


    /// <summary>
    /// CLSID_StreamBufferConfig
    /// </summary>
    [ComImport, Guid("FA8A68B2-C864-4ba2-AD53-D3876A87494B")]
    public class StreamBufferConfig
    {
    }


    /// <summary>
    /// CLSID_StreamBufferComposeRecording
    /// </summary>
    [ComImport, Guid("D682C4BA-A90A-42fe-B9E1-03109849C423")]
    public class StreamBufferComposeRecording
    {
    }


    /// <summary>
    /// CLSID_SeekingPassThru
    /// </summary>
    [ComImport, Guid("060AF76C-68DD-11d0-8FC1-00C04FD9189D")]
    public class SeekingPassThru
    {
    }


    /// <summary>
    /// CLSID_FilterMapper2
    /// </summary>
    [ComImport, Guid("CDA42200-BD88-11d0-BD4E-00A0C911CE86")]
    public class FilterMapper2
    {
    }


    /// <summary>
    /// CLSID_MemoryAllocator
    /// </summary>
    [ComImport, Guid("1e651cc0-b199-11d0-8212-00c04fc32c45")]
    public class MemoryAllocator
    {
    }


    /// <summary>
    /// CLSID_MediaPropertyBag
    /// </summary>
    [ComImport, Guid("CDBD8D00-C193-11d0-BD4E-00A0C911CE86")]
    public class MediaPropertyBag
    {
    }


    /// <summary>
    /// CLSID_DVDState
    /// </summary>
    [ComImport, Guid("f963c5cf-a659-4a93-9638-caf3cd277d13")]
    public class DVDState
    {
    }

    /// <summary>
    /// CLSID_SectionList
    /// </summary>
    [ComImport, Guid("73DA5D04-4347-45d3-A9DC-FAE9DDBE558D")]
    public class SectionList
    {
    }

    /// <summary>
    /// CLSID_Mpeg2Stream
    /// </summary>
    [ComImport, Guid("F91D96C7-8509-4d0b-AB26-A0DD10904BB7")]
    public class Mpeg2Stream
    {
    }

    /// <summary>
    /// CLSID_Mpeg2Data
    /// </summary>
    [ComImport, Guid("C666E115-BB62-4027-A113-82D643FE2D99")]
    public class Mpeg2Data
    {
    }

    /// <summary>
    /// Unnamed clsid
    /// </summary>
    [ComImport, Guid("33facfe0-a9be-11d0-a520-00a0d10129c0")]
    public class SAMIParser
    {
    }

    /// <summary>
    /// Unnamed clsid
    /// </summary>
    [ComImport, Guid("48025243-2D39-11CE-875D-00608CB78066")]
    public class InternalScriptCommandRenderer
    {
    }

    /// <summary>
    /// CLSID_BroadcastEventService
    /// </summary>
    [ComImport, Guid("0B3FFB92-0919-4934-9D5B-619C719D0202")]
    public class BroadcastEventService
    {
    }

    /// <summary>
    /// CLSID_AtscPsipParser
    /// </summary>
    [ComImport, Guid("3508C064-B94E-420b-A821-20C8096FAADC")]
    public class AtscPsipParser
    {
    }

    /// <summary>
    /// CLSID_DvbSiParser
    /// </summary>
    [ComImport, Guid("F6B96EDA-1A94-4476-A85F-4D3DC7B39C3F")]
    public class DvbSiParser
    {
    }

    #endregion

    #region Filter Classes

    /// <summary>
    /// CLSID_DMOWrapperFilter
    /// </summary>
    [ComImport, Guid("94297043-bd82-4dfd-b0de-8177739c6d20")]
    public class DMOWrapperFilter
    {
    }


    /// <summary>
    /// CLSID_StreamBufferSink
    /// </summary>
    [ComImport, Guid("2DB47AE5-CF39-43c2-B4D6-0CD8D90946F4")]
    public class StreamBufferSink
    {
    }


    /// <summary>
    /// CLSID_SampleGrabber
    /// </summary>
    [ComImport, Guid("C1F400A0-3F08-11d3-9F0B-006008039E37")]
    public class SampleGrabber
    {
    }


    /// <summary>
    /// CLSID_StreamBufferSource
    /// </summary>
    [ComImport, Guid("C9F5FE02-F851-4eb5-99EE-AD602AF1E619")]
    public class StreamBufferSource
    {
    }


    /// <summary>
    /// CLSID_VideoMixingRenderer
    /// </summary>
    [ComImport, Guid("B87BEB7B-8D29-423f-AE4D-6582C10175AC")]
    public class VideoMixingRenderer
    {
    }


    /// <summary>
    /// CLSID_VideoMixingRenderer9
    /// </summary>
    [ComImport, Guid("51b4abf3-748f-4e3b-a276-c828330e926a")]
    public class VideoMixingRenderer9
    {
    }


    /// <summary>
    /// CLSID_VideoRendererDefault
    /// </summary>
    [ComImport, Guid("6BC1CFFA-8FC1-4261-AC22-CFB4CC38DB50")]
    public class VideoRendererDefault
    {
    }


    /// <summary>
    /// CLSID_AviSplitter
    /// </summary>
    [ComImport, Guid("1b544c20-fd0b-11ce-8c63-00aa0044b51e")]
    public class AviSplitter
    {
    }


    /// <summary>
    /// CLSID_SmartTee
    /// </summary>
    [ComImport, Guid("CC58E280-8AA1-11d1-B3F1-00AA003761C5")]
    public class SmartTee
    {
    }


    /// <summary>
    /// CLSID_NullRenderer
    /// </summary>
    [ComImport, Guid("C1F400A4-3F08-11d3-9F0B-006008039E37")]
    public class NullRenderer
    {
    }


    /// <summary>
    /// CLSID_ACMWrapper
    /// </summary>
    [ComImport, Guid("6a08cf80-0e18-11cf-a24d-0020afd79767")]
    public class ACMWrapper
    {
    }


    /// <summary>
    /// CLSID_AudioRender
    /// </summary>
    [ComImport, Guid("e30629d1-27e5-11ce-875d-00608cb78066")]
    public class AudioRender
    {
    }


    /// <summary>
    /// CLSID_AVIDec
    /// </summary>
    [ComImport, Guid("CF49D4E0-1115-11ce-B03A-0020AF0BA770")]
    public class AVIDec
    {
    }


    /// <summary>
    /// CLSID_AVIDraw
    /// </summary>
    [ComImport, Guid("A888DF60-1E90-11cf-AC98-00AA004C0FA9")]
    public class AVIDraw
    {
    }


    /// <summary>
    /// CLSID_AviDest
    /// </summary>
    [ComImport, Guid("E2510970-F137-11CE-8B67-00AA00A3F1A6")]
    public class AviDest
    {
    }


    /// <summary>
    /// CLSID_ATSCNetworkProvider
    /// </summary>
    [ComImport, Guid("0DAD2FDD-5FD7-11D3-8F50-00C04F7971E2")]
    public class ATSCNetworkProvider
    {
    }


    /// <summary>
    /// CLSID_DVBCNetworkProvider
    /// </summary>
    [ComImport, Guid("DC0C0FE7-0485-4266-B93F-68FBF80ED834")]
    public class DVBCNetworkProvider
    {
    }


    /// <summary>
    /// CLSID_DVBSNetworkProvider
    /// </summary>
    [ComImport, Guid("FA4B375A-45B4-4d45-8440-263957B11623")]
    public class DVBSNetworkProvider
    {
    }


    /// <summary>
    /// CLSID_DVBTNetworkProvider
    /// </summary>
    [ComImport, Guid("216C62DF-6D7F-4e9a-8571-05F14EDB766A")]
    public class DVBTNetworkProvider
    {
    }

    /// <summary>
    /// CLSID_NetworkProvider
    /// </summary>
    [ComImport, Guid("B2F3A67C-29DA-4c78-8831-091ED509A475")]
    public class NetworkProvider
    {
    }

    /// <summary>
    /// CLSID_Colour
    /// </summary>
    [ComImport, Guid("1643e180-90f5-11ce-97d5-00aa0055595a")]
    public class Colour
    {
    }


    /// <summary>
    /// CLSID_DSoundRender
    /// </summary>
    [ComImport, Guid("79376820-07D0-11cf-A24D-0020AFD79767")]
    public class DSoundRender
    {
    }


    /// <summary>
    /// CLSID_DVMux
    /// </summary>
    [ComImport, Guid("129D7E40-C10D-11d0-AFB9-00AA00B67A42")]
    public class DVMux
    {
    }


    /// <summary>
    /// CLSID_DVSplitter
    /// </summary>
    [ComImport, Guid("4EB31670-9FC6-11cf-AF6E-00AA00B67A42")]
    public class DVSplitter
    {
    }


    /// <summary>
    /// CLSID_DVVideoCodec
    /// </summary>
    [ComImport, Guid("B1B77C00-C3E4-11cf-AF79-00AA00B67A42")]
    public class DVVideoCodec
    {
    }


    /// <summary>
    /// CLSID_DVVideoEnc
    /// </summary>
    [ComImport, Guid("13AA3650-BB6F-11d0-AFB9-00AA00B67A42")]
    public class DVVideoEnc
    {
    }


    /// <summary>
    /// CLSID_DVDNavigator
    /// </summary>
    [ComImport, Guid("9B8C4620-2C1A-11d0-8493-00A02438AD48")]
    public class DVDNavigator
    {
    }


    /// <summary>
    /// CLSID_AsyncReader
    /// </summary>
    [ComImport, Guid("e436ebb5-524f-11ce-9f53-0020af0ba770")]
    public class AsyncReader
    {
    }


    /// <summary>
    /// CLSID_URLReader
    /// </summary>
    [ComImport, Guid("e436ebb6-524f-11ce-9f53-0020af0ba770")]
    public class URLReader
    {
    }


    /// <summary>
    /// CLSID_FileWriter
    /// </summary>
    [ComImport, Guid("8596E5F0-0DA5-11d0-BD21-00A0C911CE86")]
    public class FileWriter
    {
    }


    /// <summary>
    /// CLSID_ModexRenderer
    /// </summary>
    [ComImport, Guid("07167665-5011-11cf-BF33-00AA0055595A")]
    public class ModexRenderer
    {
    }


    /// <summary>
    /// CLSID_InfTee
    /// </summary>
    [ComImport, Guid("F8388A40-D5BB-11d0-BE5A-0080C706568E")]
    public class InfTee
    {
    }


    /// <summary>
    /// CLSID_Line21Decoder
    /// </summary>
    [ComImport, Guid("6E8D4A20-310C-11d0-B79A-00AA003767A7")]
    public class Line21Decoder
    {
    }


    /// <summary>
    /// CLSID_Line21Decoder2
    /// </summary>
    [ComImport, Guid("E4206432-01A1-4BEE-B3E1-3702C8EDC574")]
    public class Line21Decoder2
    {
    }


    /// <summary>
    /// CLSID_AVIMIDIRender
    /// </summary>
    [ComImport, Guid("07b65360-c445-11ce-afde-00aa006c14f4")]
    public class AVIMIDIRender
    {
    }


    /// <summary>
    /// CLSID_MJPGEnc
    /// </summary>
    [ComImport, Guid("B80AB0A0-7416-11d2-9EEB-006008039E37")]
    public class MJPGEnc
    {
    }


    /// <summary>
    /// CLSID_MjpegDec
    /// </summary>
    [ComImport, Guid("301056D0-6DFF-11d2-9EEB-006008039E37")]
    public class MjpegDec
    {
    }


    /// <summary>
    /// CLSID_CMpegAudioCodec
    /// </summary>
    [ComImport, Guid("4a2286e0-7bef-11ce-9bd9-0000e202599c")]
    public class CMpegAudioCodec
    {
    }


    /// <summary>
    /// CLSID_MPEG1Splitter
    /// </summary>
    [ComImport, Guid("336475d0-942a-11ce-a870-00aa002feab5")]
    public class MPEG1Splitter
    {
    }


    /// <summary>
    /// CLSID_CMpegVideoCodec
    /// </summary>
    [ComImport, Guid("feb50740-7bef-11ce-9bd9-0000e202599c")]
    public class CMpegVideoCodec
    {
    }


    /// <summary>
    /// CLSID_MPEG2Demultiplexer
    /// </summary>
    [ComImport, Guid("afb6c280-2c41-11d3-8a60-0000f81e0e4a")]
    public class MPEG2Demultiplexer
    {
    }


    /// <summary>
    /// CLSID_MMSPLITTER
    /// </summary>
    [ComImport, Guid("3ae86b20-7be8-11d1-abe6-00a0c905f375")]
    public class MMSPLITTER
    {
    }


    /// <summary>
    /// CLSID_OverlayMixer
    /// </summary>
    [ComImport, Guid("CD8743A1-3736-11d0-9E69-00C04FD7C15B")]
    public class OverlayMixer
    {
    }

    /// <summary>
    /// CLSID_OverlayMixer2
    /// </summary>
    [ComImport, Guid("A0025E90-E45B-11D1-ABE9-00A0C905F375")]
    public class OverlayMixer2
    {
    }


    /// <summary>
    /// CLSID_QTDec
    /// </summary>
    [ComImport, Guid("FDFE9681-74A3-11d0-AFA7-00AA00B67A42")]
    public class QTDec
    {
    }


    /// <summary>
    /// CLSID_QuickTimeParser
    /// </summary>
    [ComImport, Guid("D51BD5A0-7548-11cf-A520-0080C77EF58A")]
    public class QuickTimeParser
    {
    }


    /// <summary>
    /// CLSID_VBISurfaces
    /// </summary>
    [ComImport, Guid("814B9800-1C88-11d1-BAD9-00609744111A")]
    public class VBISurfaces
    {
    }


    /// <summary>
    /// CLSID_VfwCapture
    /// </summary>
    [ComImport, Guid("1b544c22-fd0b-11ce-8c63-00aa0044b51e")]
    public class VfwCapture
    {
    }


    /// <summary>
    /// CLSID_Dither
    /// </summary>
    [ComImport, Guid("1da08500-9edc-11cf-bc10-00aa00ac74f6")]
    public class Dither
    {
    }


    /// <summary>
    /// CLSID_VideoPortManager
    /// </summary>
    [ComImport, Guid("6f26a6cd-967b-47fd-874a-7aed2c9d25a2")]
    public class VideoPortManager
    {
    }


    /// <summary>
    /// CLSID_VideoRenderer
    /// </summary>
    [ComImport, Guid("70e102b0-5556-11ce-97c0-00aa0055595a")]
    public class VideoRenderer
    {
    }


    /// <summary>
    /// CLSID_WMAsfReader
    /// </summary>
    [ComImport, Guid("187463A0-5BB7-11d3-ACBE-0080C75E246E")]
    public class WMAsfReader
    {
    }


    /// <summary>
    /// CLSID_SystemClock
    /// </summary>
    [ComImport, Guid("e436ebb1-524f-11ce-9f53-0020af0ba770")]
    public class SystemClock
    {
    }


    /// <summary>
    /// CLSID_WMAsfWriter
    /// </summary>
    [ComImport, Guid("7c23220e-55bb-11d3-8b16-00c04fb6bd3d")]
    public class WMAsfWriter
    {
    }


    /// <summary>
    /// CLSID_WSTDecoder
    /// </summary>
    [ComImport, Guid("70BC06E0-5666-11d3-A184-00105AEF9F33")]
    public class WSTDecoder
    {
    }


    /// <summary>
    /// CLSID_Mpeg2VideoStreamAnalyzer
    /// </summary>
    [ComImport, Guid("6CFAD761-735D-4aa5-8AFC-AF91A7D61EBA")]
    public class Mpeg2VideoStreamAnalyzer
    {
    }


    /// <summary>
    /// CLSID_PTFilter
    /// </summary>
    [ComImport, Guid("9CD31617-B303-4f96-8330-2EB173EA4DC6")]
    public class PTFilter
    {
    }

    /// <summary>
    /// CLSID_MPEG2Demultiplexer_NoClock
    /// </summary>
    [ComImport, Guid("687D3367-3644-467a-ADFE-6CD7A85C4A2C")]
    public class MPEG2Demultiplexer_NoClock
    {
    }


    /// <summary>
    /// CLSID_SBE2Sink
    /// </summary>
    [ComImport, Guid("E2448508-95DA-4205-9A27-7EC81E723B1A")]
    public class SBE2Sink
    {
    }


    /// <summary>
    /// CLSID_StreamBufferPropertyHandler
    /// </summary>
    [ComImport, Guid("E37A73F8-FB01-43dc-914E-AAEE76095AB9")]
    public class StreamBufferPropertyHandler
    {
    }


    /// <summary>
    /// CLSID_StreamBufferThumbnailHandler
    /// </summary>
    [ComImport, Guid("713790EE-5EE1-45ba-8070-A1337D2762FA")]
    public class StreamBufferThumbnailHandler
    {
    }


    /// <summary>
    /// CLSID_SBE2File
    /// </summary>
    [ComImport, Guid("93A094D7-51E8-485b-904A-8D6B97DC6B39")]
    public class SBE2File
    {
    }


    /// <summary>
    /// CLSID_CCAFilter
    /// </summary>
    [ComImport, Guid("3d07a539-35ca-447c-9b05-8d85ce924f9e")]
    public class CCAFilter
    {
    }


    /// <summary>
    /// CLSID_CaptionsFilter
    /// </summary>
    [ComImport, Guid("2F7EE4B6-6FF5-4EB4-B24A-2BFC41117171")]
    public class CaptionsFilter
    {
    }


    /// <summary>
    /// CLSID_SubtitlesFilter
    /// </summary>
    [ComImport, Guid("9F22CFEA-CE07-41ab-8BA0-C7364AF90AF9")]
    public class SubtitlesFilter
    {
    }


    /// <summary>
    /// CLSID_DirectShowPluginControl
    /// </summary>
    [ComImport, Guid("8670C736-F614-427b-8ADA-BBADC587194B")]
    public class DirectShowPluginControl
    {
    }


    /// <summary>
    /// CLSID_SBE2MediaTypeProfile
    /// </summary>
    [ComImport, Guid("1f26a602-2b5c-4b63-b8e8-9ea5c1a7dc2e")]
    public class SBE2MediaTypeProfile
    {
    }


    /// <summary>
    /// CLSID_SBE2FileScan
    /// </summary>
    [ComImport, Guid("3E458037-0CA6-41aa-A594-2AA6C02D709B")]
    public class SBE2FileScan
    {
    }


    #endregion

    #region GUIDS

    static public class FilterCategory
    {
        /// <summary> CLSID_CPCAFiltersCategory </summary>
        public static readonly Guid CPCAFiltersCategory = new Guid(0xC4C4C4FC, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> CLSID_MediaEncoderCategory </summary>
        public static readonly Guid MediaEncoderCategory = new Guid(0x7D22E920, 0x5CA9, 0x4787, 0x8C, 0x2B, 0xA6, 0x77, 0x9B, 0xD1, 0x17, 0x81);

        /// <summary> CLSID_MediaMultiplexerCategory </summary>
        public static readonly Guid MediaMultiplexerCategory = new Guid(0x236C9559, 0xADCE, 0x4736, 0xBF, 0x72, 0xBA, 0xB3, 0x4E, 0x39, 0x21, 0x96);

        /// <summary> CLSID_DMOFilterCategory </summary>
        public static readonly Guid DMOFilterCategory = new Guid(0xbcd5796c, 0xbd52, 0x4d30, 0xab, 0x76, 0x70, 0xf9, 0x75, 0xb8, 0x91, 0x99);

        /// <summary> CLSID_AudioInputDeviceCategory, audio capture category </summary>
        public static readonly Guid AudioInputDevice = new Guid(0x33d9a762, 0x90c8, 0x11d0, 0xbd, 0x43, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> CLSID_VideoInputDeviceCategory, video capture category </summary>
        public static readonly Guid VideoInputDevice = new Guid(0x860BB310, 0x5D01, 0x11d0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        /// <summary> CLSID_VideoCompressorCategory, video compressor category </summary>
        public static readonly Guid VideoCompressorCategory = new Guid(0x33d9a760, 0x90c8, 0x11d0, 0xbd, 0x43, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> CLSID_AudioCompressorCategory, audio compressor category </summary>
        public static readonly Guid AudioCompressorCategory = new Guid(0x33d9a761, 0x90c8, 0x11d0, 0xbd, 0x43, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> CLSID_LegacyAmFilterCategory, legacy filters </summary>
        public static readonly Guid LegacyAmFilterCategory = new Guid(0x083863F1, 0x70DE, 0x11d0, 0xBD, 0x40, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        /// <summary> CLSID_AudioRendererCategory, Audio renderer category</summary>
        public static readonly Guid AudioRendererCategory = new Guid(0xe0f158e1, 0xcb04, 0x11d0, 0xbd, 0x4e, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> KSCATEGORY_BDA_RECEIVER_COMPONENT, BDA Receiver Components category</summary>
        public static readonly Guid BDAReceiverComponentsCategory = new Guid(0xFD0A5AF4, 0xB41D, 0x11d2, 0x9c, 0x95, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSCATEGORY_BDA_NETWORK_TUNER, BDA Source Filters category</summary>
        public static readonly Guid BDASourceFiltersCategory = new Guid(0x71985f48, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSCATEGORY_BDA_IP_SINK, BDA Rendering Filters category</summary>
        public static readonly Guid BDARenderingFiltersCategory = new Guid(0x71985f4a, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSCATEGORY_BDA_NETWORK_PROVIDER, BDA Network Providers category</summary>
        public static readonly Guid BDANetworkProvidersCategory = new Guid(0x71985f4b, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSCATEGORY_BDA_TRANSPORT_INFORMATION, BDA Transport Information Renderers category</summary>
        public static readonly Guid BDATransportInformationRenderersCategory = new Guid(0xa2e3074f, 0x6c3d, 0x11d3, 0xb6, 0x53, 0x00, 0xc0, 0x4f, 0x79, 0x49, 0x8e);

        /// <summary> CLSID_MidiRendererCategory </summary>
        public static readonly Guid MidiRendererCategory = new Guid(0x4EfE2452, 0x168A, 0x11d1, 0xBC, 0x76, 0x00, 0xc0, 0x4F, 0xB9, 0x45, 0x3B);

        /// <summary> CLSID_TransmitCategory  External Renderers Category</summary>
        public static readonly Guid TransmitCategory = new Guid(0xcc7bfb41, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_DeviceControlCategory  Device Control Filters</summary>
        public static readonly Guid DeviceControlCategory = new Guid(0xcc7bfb46, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_VideoEffects1Category </summary>
        public static readonly Guid VideoEffects1Category = new Guid(0xcc7bfb42, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_VideoEffects2Category </summary>
        public static readonly Guid VideoEffects2Category = new Guid(0xcc7bfb43, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_AudioEffects1Category </summary>
        public static readonly Guid AudioEffects1Category = new Guid(0xcc7bfb44, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_AudioEffects2Category </summary>
        public static readonly Guid AudioEffects2Category = new Guid(0xcc7bfb45, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> KSCATEGORY_DATADECOMPRESSOR & CLSID_DVDHWDecodersCategory</summary>
        public static readonly Guid KSDataDecompressor = new Guid(0x2721AE20, 0x7E70, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> KSCATEGORY_COMMUNICATIONSTRANSFORM </summary>
        public static readonly Guid KSCommunicationsTransform = new Guid(0xCF1DDA2C, 0x9743, 0x11D0, 0xA3, 0xEE, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> KSCATEGORY_DATATRANSFORM </summary>
        public static readonly Guid KSDataTransform = new Guid(0x2EB07EA0, 0x7E70, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> KSCATEGORY_INTERFACETRANSFORM </summary>
        public static readonly Guid KSInterfaceTransform = new Guid(0xCF1DDA2D, 0x9743, 0x11D0, 0xA3, 0xEE, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> KSCATEGORY_MIXER </summary>
        public static readonly Guid KSMixer = new Guid(0xAD809C00, 0x7B88, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> KSCATEGORY_AUDIO_DEVICE </summary>
        public static readonly Guid KSAudioDevice = new Guid(0xFBF6F530, 0x07B9, 0x11D2, 0xA7, 0x1E, 0x00, 0x00, 0xF8, 0x00, 0x47, 0x88);

        /// <summary> CLSID_ActiveMovieCategories </summary>
        public static readonly Guid ActiveMovieCategories = new Guid(0xda4e3da0, 0xd07d, 0x11d0, 0xbd, 0x50, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> AM_KSCATEGORY_CAPTURE </summary>
        public static readonly Guid AMKSCapture = new Guid(0x65E8773D, 0x8F56, 0x11D0, 0xA3, 0xB9, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> AM_KSCATEGORY_RENDER </summary>
        public static readonly Guid AMKSRender = new Guid(0x65E8773E, 0x8F56, 0x11D0, 0xA3, 0xB9, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> AM_KSCATEGORY_DATACOMPRESSOR </summary>
        public static readonly Guid AMKSDataCompressor = new Guid(0x1E84C900, 0x7E70, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> AM_KSCATEGORY_AUDIO </summary>
        public static readonly Guid AMKSAudio = new Guid(0x6994AD04, 0x93EF, 0x11D0, 0xA3, 0xCC, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> AM_KSCATEGORY_VIDEO </summary>
        public static readonly Guid AMKSVideo = new Guid(0x6994AD05, 0x93EF, 0x11D0, 0xA3, 0xCC, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> AM_KSCATEGORY_TVTUNER </summary>
        public static readonly Guid AMKSTVTuner = new Guid(0xa799a800, 0xa46d, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0x24, 0x01, 0xdc, 0xd4);

        /// <summary> AM_KSCATEGORY_CROSSBAR </summary>
        public static readonly Guid AMKSCrossbar = new Guid(0xa799a801, 0xa46d, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0x24, 0x01, 0xdc, 0xd4);

        /// <summary> AM_KSCATEGORY_TVAUDIO </summary>
        public static readonly Guid AMKSTVAudio = new Guid(0xa799a802, 0xa46d, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0x24, 0x01, 0xdc, 0xd4);

        /// <summary> AM_KSCATEGORY_VBICODEC </summary>
        public static readonly Guid AMKSVBICodec = new Guid(0x07dad660, 0x22f1, 0x11d1, 0xa9, 0xf4, 0x00, 0xc0, 0x4f, 0xbb, 0xde, 0x8f);

        /// <summary> AM_KSCATEGORY_SPLITTER </summary>
        public static readonly Guid AMKSSplitter = new Guid(0x0A4252A0, 0x7E70, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> Not defined </summary>
        public static readonly Guid WDMStreamingEncoderDevices = new Guid(0x19689BF6, 0xC384, 0x48FD, 0xAD, 0x51, 0x90, 0xE5, 0x8C, 0x79, 0xF7, 0x0B);

        /// <summary> Not defined </summary>
        public static readonly Guid WDMStreamingMultiplexerDevices = new Guid(0x7A5DE1D3, 0x01A1, 0x452C, 0xB4, 0x81, 0x4F, 0xA2, 0xB9, 0x62, 0x71, 0xE8);

        /// <summary> Not defined </summary>
        public static readonly Guid LTMMVideoProcessors = new Guid(0xE526D606, 0x22E7, 0x494C, 0xB8, 0x1E, 0xAC, 0x0A, 0x94, 0xBF, 0xE6, 0x03);
    }

    static public class VMRClsId
    {
        /// <summary>CLSID_AllocPresenter</summary>
        public static readonly Guid AllocPresenter = new Guid(0x99d54f63, 0x1a69, 0x41ae, 0xaa, 0x4d, 0xc9, 0x76, 0xeb, 0x3f, 0x07, 0x13);

        /// <summary>CLSID_AllocPresenterDDXclMode</summary>
        public static readonly Guid AllocPresenterDDXclMode = new Guid(0x4444ac9e, 0x242e, 0x471b, 0xa3, 0xc7, 0x45, 0xdc, 0xd4, 0x63, 0x52, 0xbc);
    }

    static public class TVEClsId
    {
        /// <summary>CLSID_DShowTVEFilter</summary>
        public static readonly Guid DShowTVEFilter = new Guid(0x05500280, 0xFAA5, 0x4DF9, 0x82, 0x46, 0xBF, 0xC2, 0x3A, 0xC5, 0xCE, 0xA8);

        /// <summary>CLSID_TVEFilterTuneProperties</summary>
        public static readonly Guid TVEFilterTuneProperties = new Guid(0x05500281, 0xFAA5, 0x4DF9, 0x82, 0x46, 0xBF, 0xC2, 0x3A, 0xC5, 0xCE, 0xA8);

        /// <summary>CLSID_TVEFilterCCProperties</summary>
        public static readonly Guid TVEFilterCCProperties = new Guid(0x05500282, 0xFAA5, 0x4DF9, 0x82, 0x46, 0xBF, 0xC2, 0x3A, 0xC5, 0xCE, 0xA8);

        /// <summary>CLSID_TVEFilterStatsProperties</summary>
        public static readonly Guid TVEFilterStatsProperties = new Guid(0x05500283, 0xFAA5, 0x4DF9, 0x82, 0x46, 0xBF, 0xC2, 0x3A, 0xC5, 0xCE, 0xA8);
    }

    static public class ENCAPIClsId
    {
        /// <summary>CLSID_IVideoEncoderProxy</summary>
        public static readonly Guid IVideoEncoderProxy = new Guid(0xb43c4eec, 0x8c32, 0x4791, 0x91, 0x02, 0x50, 0x8a, 0xda, 0x5e, 0xe8, 0xe7);

        /// <summary>CLSID_ICodecAPIProxy</summary>
        public static readonly Guid ICodecAPIProxy = new Guid(0x7ff0997a, 0x1999, 0x4286, 0xa7, 0x3c, 0x62, 0x2b, 0x88, 0x14, 0xe7, 0xeb);

        /// <summary>CLSID_IVideoEncoderCodecAPIProxy</summary>
        public static readonly Guid IVideoEncoderCodecAPIProxy = new Guid(0xb05dabd9, 0x56e5, 0x4fdc, 0xaf, 0xa4, 0x8a, 0x47, 0xe9, 0x1f, 0x1c, 0x9c);
    }

    static public class MediaType
    {
        public static readonly Guid Null = Guid.Empty;

        /// <summary> MEDIATYPE_Video 'vids' </summary>
        public static readonly Guid Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Interleaved 'iavs' </summary>
        public static readonly Guid Interleaved = new Guid(0x73766169, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Audio 'auds' </summary>
        public static readonly Guid Audio = new Guid(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Text 'txts' </summary>
        public static readonly Guid Texts = new Guid(0x73747874, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Stream </summary>
        public static readonly Guid Stream = new Guid(0xe436eb83, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIATYPE_VBI </summary>
        public static readonly Guid VBI = new Guid(0xf72a76e1, 0xeb0a, 0x11d0, 0xac, 0xe4, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> MEDIATYPE_Midi </summary>
        public static readonly Guid Midi = new Guid(0x7364696D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_File </summary>
        public static readonly Guid File = new Guid(0x656c6966, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_ScriptCommand </summary>
        public static readonly Guid ScriptCommand = new Guid(0x73636d64, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_AUXLine21Data </summary>
        public static readonly Guid AuxLine21Data = new Guid(0x670aea80, 0x3a82, 0x11d0, 0xb7, 0x9b, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIATYPE_Timecode </summary>
        public static readonly Guid Timecode = new Guid(0x0482dee3, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_LMRT </summary>
        public static readonly Guid LMRT = new Guid(0x74726c6d, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_URL_STREAM </summary>
        public static readonly Guid URLStream = new Guid(0x736c7275, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_AnalogVideo </summary>
        public static readonly Guid AnalogVideo = new Guid(0x0482dde1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_AnalogAudio </summary>
        public static readonly Guid AnalogAudio = new Guid(0x0482dee1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_MPEG2_SECTIONS </summary>
        public static readonly Guid Mpeg2Sections = new Guid(0x455f176c, 0x4b06, 0x47ce, 0x9a, 0xef, 0x8c, 0xae, 0xf7, 0x3d, 0xf7, 0xb5);

        /// <summary> MEDIATYPE_DTVCCData </summary>
        public static readonly Guid DTVCCData = new Guid(0xfb77e152, 0x53b2, 0x499c, 0xb4, 0x6b, 0x50, 0x9f, 0xc3, 0x3e, 0xdf, 0xd7);

        /// <summary> MEDIATYPE_MSTVCaption </summary>
        public static readonly Guid MSTVCaption = new Guid(0xB88B8A89, 0xB049, 0x4C80, 0xAD, 0xCF, 0x58, 0x98, 0x98, 0x5E, 0x22, 0xC1);

        /// <summary> MEDIATYPE_AUXTeletextPage </summary>
        public static readonly Guid AUXTeletextPage = new Guid(0x11264acb, 0x37de, 0x4eba, 0x8c, 0x35, 0x7f, 0x4, 0xa1, 0xa6, 0x83, 0x32);

        /// <summary> MEDIATYPE_CC_CONTAINER </summary>
        public static readonly Guid CC_Container = new Guid(0xaeb312e9, 0x3357, 0x43ca, 0xb7, 0x1, 0x97, 0xec, 0x19, 0x8e, 0x2b, 0x62);

    }

    static public class MediaSubType
    {
        public static readonly Guid Null = Guid.Empty;

        /// <summary> MEDIASUBTYPE_CLPL </summary>
        public static readonly Guid CLPL = new Guid(0x4C504C43, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YUYV </summary>
        public static readonly Guid YUYV = new Guid(0x56595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IYUV </summary>
        public static readonly Guid IYUV = new Guid(0x56555949, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YVU9 </summary>
        public static readonly Guid YVU9 = new Guid(0x39555659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y411 </summary>
        public static readonly Guid Y411 = new Guid(0x31313459, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y41P </summary>
        public static readonly Guid Y41P = new Guid(0x50313459, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YUY2 </summary>
        public static readonly Guid YUY2 = new Guid(0x32595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YVYU </summary>
        public static readonly Guid YVYU = new Guid(0x55595659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_UYVY </summary>
        public static readonly Guid UYVY = new Guid(0x59565955, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y211 </summary>
        public static readonly Guid Y211 = new Guid(0x31313259, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CLJR </summary>
        public static readonly Guid CLJR = new Guid(0x524a4c43, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IF09 </summary>
        public static readonly Guid IF09 = new Guid(0x39304649, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CPLA </summary>
        public static readonly Guid CPLA = new Guid(0x414c5043, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_MJPG </summary>
        public static readonly Guid MJPG = new Guid(0x47504A4D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_TVMJ </summary>
        public static readonly Guid TVMJ = new Guid(0x4A4D5654, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_WAKE </summary>
        public static readonly Guid WAKE = new Guid(0x454B4157, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CFCC </summary>
        public static readonly Guid CFCC = new Guid(0x43434643, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IJPG </summary>
        public static readonly Guid IJPG = new Guid(0x47504A49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Plum </summary>
        public static readonly Guid PLUM = new Guid(0x6D756C50, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DVCS </summary>
        public static readonly Guid DVCS = new Guid(0x53435644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DVSD </summary>
        public static readonly Guid DVSD = new Guid(0x44535644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_MDVF </summary>
        public static readonly Guid MDVF = new Guid(0x4656444D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB1 </summary>
        public static readonly Guid RGB1 = new Guid(0xe436eb78, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB4 </summary>
        public static readonly Guid RGB4 = new Guid(0xe436eb79, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB8 </summary>
        public static readonly Guid RGB8 = new Guid(0xe436eb7a, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB565 </summary>
        public static readonly Guid RGB565 = new Guid(0xe436eb7b, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB555 </summary>
        public static readonly Guid RGB555 = new Guid(0xe436eb7c, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB24 </summary>
        public static readonly Guid RGB24 = new Guid(0xe436eb7d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB32 </summary>
        public static readonly Guid RGB32 = new Guid(0xe436eb7e, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_ARGB1555 </summary>
        public static readonly Guid ARGB1555 = new Guid(0x297c55af, 0xe209, 0x4cb3, 0xb7, 0x57, 0xc7, 0x6d, 0x6b, 0x9c, 0x88, 0xa8);

        /// <summary> MEDIASUBTYPE_ARGB4444 </summary>
        public static readonly Guid ARGB4444 = new Guid(0x6e6415e6, 0x5c24, 0x425f, 0x93, 0xcd, 0x80, 0x10, 0x2b, 0x3d, 0x1c, 0xca);

        /// <summary> MEDIASUBTYPE_ARGB32 </summary>
        public static readonly Guid ARGB32 = new Guid(0x773c9ac0, 0x3274, 0x11d0, 0xb7, 0x24, 0x00, 0xaa, 0x00, 0x6c, 0x1a, 0x01);

        /// <summary> MEDIASUBTYPE_A2R10G10B10 </summary>
        public static readonly Guid A2R10G10B10 = new Guid(0x2f8bb76d, 0xb644, 0x4550, 0xac, 0xf3, 0xd3, 0x0c, 0xaa, 0x65, 0xd5, 0xc5);

        /// <summary> MEDIASUBTYPE_A2B10G10R10 </summary>
        public static readonly Guid A2B10G10R10 = new Guid(0x576f7893, 0xbdf6, 0x48c4, 0x87, 0x5f, 0xae, 0x7b, 0x81, 0x83, 0x45, 0x67);

        /// <summary> MEDIASUBTYPE_AYUV </summary>
        public static readonly Guid AYUV = new Guid(0x56555941, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_AI44 </summary>
        public static readonly Guid AI44 = new Guid(0x34344941, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IA44 </summary>
        public static readonly Guid IA44 = new Guid(0x34344149, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB32_D3D_DX7_RT </summary>
        public static readonly Guid RGB32_D3D_DX7_RT = new Guid(0x32335237, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB16_D3D_DX7_RT </summary>
        public static readonly Guid RGB16_D3D_DX7_RT = new Guid(0x36315237, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB32_D3D_DX7_RT </summary>
        public static readonly Guid ARGB32_D3D_DX7_RT = new Guid(0x38384137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB4444_D3D_DX7_RT </summary>
        public static readonly Guid ARGB4444_D3D_DX7_RT = new Guid(0x34344137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB1555_D3D_DX7_RT </summary>
        public static readonly Guid ARGB1555_D3D_DX7_RT = new Guid(0x35314137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB32_D3D_DX9_RT </summary>
        public static readonly Guid RGB32_D3D_DX9_RT = new Guid(0x32335239, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB16_D3D_DX9_RT </summary>
        public static readonly Guid RGB16_D3D_DX9_RT = new Guid(0x36315239, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB32_D3D_DX9_RT </summary>
        public static readonly Guid ARGB32_D3D_DX9_RT = new Guid(0x38384139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB4444_D3D_DX9_RT </summary>
        public static readonly Guid ARGB4444_D3D_DX9_RT = new Guid(0x34344139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB1555_D3D_DX9_RT </summary>
        public static readonly Guid ARGB1555_D3D_DX9_RT = new Guid(0x35314139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YV12 </summary>
        public static readonly Guid YV12 = new Guid(0x32315659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_NV12 </summary>
        public static readonly Guid NV12 = new Guid(0x3231564E, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC1 </summary>
        public static readonly Guid IMC1 = new Guid(0x31434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC2 </summary>
        public static readonly Guid IMC2 = new Guid(0x32434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC3 </summary>
        public static readonly Guid IMC3 = new Guid(0x33434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC4 </summary>
        public static readonly Guid IMC4 = new Guid(0x34434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_S340 </summary>
        public static readonly Guid S340 = new Guid(0x30343353, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_S342 </summary>
        public static readonly Guid S342 = new Guid(0x32343353, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Overlay </summary>
        public static readonly Guid Overlay = new Guid(0xe436eb7f, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Packet </summary>
        public static readonly Guid MPEG1Packet = new Guid(0xe436eb80, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Payload </summary>
        public static readonly Guid MPEG1Payload = new Guid(0xe436eb81, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1AudioPayload </summary>
        public static readonly Guid MPEG1AudioPayload = new Guid(0x00000050, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary> MEDIATYPE_MPEG1SystemStream </summary>
        public static readonly Guid MPEG1SystemStream = new Guid(0xe436eb82, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1System </summary>
        public static readonly Guid MPEG1System = new Guid(0xe436eb84, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1VideoCD </summary>
        public static readonly Guid MPEG1VideoCD = new Guid(0xe436eb85, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Video </summary>
        public static readonly Guid MPEG1Video = new Guid(0xe436eb86, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Audio </summary>
        public static readonly Guid MPEG1Audio = new Guid(0xe436eb87, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_Avi </summary>
        public static readonly Guid Avi = new Guid(0xe436eb88, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_Asf </summary>
        public static readonly Guid Asf = new Guid(0x3db80f90, 0x9412, 0x11d1, 0xad, 0xed, 0x00, 0x00, 0xf8, 0x75, 0x4b, 0x99);

        /// <summary> MEDIASUBTYPE_QTMovie </summary>
        public static readonly Guid QTMovie = new Guid(0xe436eb89, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_QTRpza </summary>
        public static readonly Guid QTRpza = new Guid(0x617a7072, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTSmc </summary>
        public static readonly Guid QTSmc = new Guid(0x20636d73, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTRle </summary>
        public static readonly Guid QTRle = new Guid(0x20656c72, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTJpeg </summary>
        public static readonly Guid QTJpeg = new Guid(0x6765706a, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_PCMAudio_Obsolete </summary>
        public static readonly Guid PCMAudio_Obsolete = new Guid(0xe436eb8a, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_PCM </summary>
        public static readonly Guid PCM = new Guid(0x00000001, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary> MEDIASUBTYPE_WAVE </summary>
        public static readonly Guid WAVE = new Guid(0xe436eb8b, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_AU </summary>
        public static readonly Guid AU = new Guid(0xe436eb8c, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_AIFF </summary>
        public static readonly Guid AIFF = new Guid(0xe436eb8d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_dvhd </summary>
        public static readonly Guid dvhd = new Guid(0x64687664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dvsl </summary>
        public static readonly Guid dvsl = new Guid(0x6c737664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dv25 </summary>
        public static readonly Guid dv25 = new Guid(0x35327664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dv50 </summary>
        public static readonly Guid dv50 = new Guid(0x30357664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dvh1 </summary>
        public static readonly Guid dvh1 = new Guid(0x31687664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Line21_BytePair </summary>
        public static readonly Guid Line21_BytePair = new Guid(0x6e8d4a22, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_Line21_GOPPacket </summary>
        public static readonly Guid Line21_GOPPacket = new Guid(0x6e8d4a23, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_Line21_VBIRawData </summary>
        public static readonly Guid Line21_VBIRawData = new Guid(0x6e8d4a24, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_TELETEXT </summary>
        public static readonly Guid TELETEXT = new Guid(0xf72a76e3, 0xeb0a, 0x11d0, 0xac, 0xe4, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> MEDIASUBTYPE_WSS </summary>
        public static readonly Guid WSS = new Guid(0x2791D576, 0x8E7A, 0x466F, 0x9E, 0x90, 0x5D, 0x3F, 0x30, 0x83, 0x73, 0x8B);

        /// <summary> MEDIASUBTYPE_VPS </summary>
        public static readonly Guid VPS = new Guid(0xa1b3f620, 0x9792, 0x4d8d, 0x81, 0xa4, 0x86, 0xaf, 0x25, 0x77, 0x20, 0x90);

        /// <summary> MEDIASUBTYPE_DRM_Audio </summary>
        public static readonly Guid DRM_Audio = new Guid(0x00000009, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IEEE_FLOAT </summary>
        public static readonly Guid IEEE_FLOAT = new Guid(0x00000003, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DOLBY_AC3_SPDIF </summary>
        public static readonly Guid DOLBY_AC3_SPDIF = new Guid(0x00000092, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RAW_SPORT </summary>
        public static readonly Guid RAW_SPORT = new Guid(0x00000240, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_SPDIF_TAG_241h </summary>
        public static readonly Guid SPDIF_TAG_241h = new Guid(0x00000241, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DssVideo </summary>
        public static readonly Guid DssVideo = new Guid(0xa0af4f81, 0xe163, 0x11d0, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_DssAudio </summary>
        public static readonly Guid DssAudio = new Guid(0xa0af4f82, 0xe163, 0x11d0, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_VPVideo </summary>
        public static readonly Guid VPVideo = new Guid(0x5a9b6a40, 0x1a22, 0x11d1, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_VPVBI </summary>
        public static readonly Guid VPVBI = new Guid(0x5a9b6a41, 0x1a22, 0x11d1, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_AnalogVideo_NTSC_M </summary>
        public static readonly Guid AnalogVideo_NTSC_M = new Guid(0x0482dde2, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_B </summary>
        public static readonly Guid AnalogVideo_PAL_B = new Guid(0x0482dde5, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_D </summary>
        public static readonly Guid AnalogVideo_PAL_D = new Guid(0x0482dde6, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_G </summary>
        public static readonly Guid AnalogVideo_PAL_G = new Guid(0x0482dde7, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_H </summary>
        public static readonly Guid AnalogVideo_PAL_H = new Guid(0x0482dde8, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_I </summary>
        public static readonly Guid AnalogVideo_PAL_I = new Guid(0x0482dde9, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_M </summary>
        public static readonly Guid AnalogVideo_PAL_M = new Guid(0x0482ddea, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_N </summary>
        public static readonly Guid AnalogVideo_PAL_N = new Guid(0x0482ddeb, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_N_COMBO </summary>
        public static readonly Guid AnalogVideo_PAL_N_COMBO = new Guid(0x0482ddec, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_B </summary>
        public static readonly Guid AnalogVideo_SECAM_B = new Guid(0x0482ddf0, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_D </summary>
        public static readonly Guid AnalogVideo_SECAM_D = new Guid(0x0482ddf1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_G </summary>
        public static readonly Guid AnalogVideo_SECAM_G = new Guid(0x0482ddf2, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_H </summary>
        public static readonly Guid AnalogVideo_SECAM_H = new Guid(0x0482ddf3, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_K </summary>
        public static readonly Guid AnalogVideo_SECAM_K = new Guid(0x0482ddf4, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_K1 </summary>
        public static readonly Guid AnalogVideo_SECAM_K1 = new Guid(0x0482ddf5, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_L </summary>
        public static readonly Guid AnalogVideo_SECAM_L = new Guid(0x0482ddf6, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> not in uuids.h </summary>
        public static readonly Guid I420    = new Guid(0x30323449, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> WMMEDIASUBTYPE_VIDEOIMAGE </summary>
        public static readonly Guid VideoImage    = new Guid(0x1d4a45f2, 0xe5f6, 0x4b44, 0x83, 0x88, 0xf0, 0xae, 0x5c, 0x0e, 0x0c, 0x37);

        /// <summary> WMMEDIASUBTYPE_MPEG2_VIDEO </summary>
        public static readonly Guid Mpeg2Video    = new Guid(0xe06d8026, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> WMMEDIASUBTYPE_WebStream </summary>
        public static readonly Guid WebStream    = new Guid(0x776257d4, 0xc627, 0x41cb, 0x8f, 0x81, 0x7a, 0xc7, 0xff, 0x1c, 0x40, 0xcc);

        /// <summary> MEDIASUBTYPE_MPEG2_AUDIO </summary>
        public static readonly Guid Mpeg2Audio = new Guid(0xe06d802b, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_DOLBY_AC3 </summary>
        public static readonly Guid DolbyAC3 = new Guid(0xe06d802c, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_DVB_SI </summary>
        public static readonly Guid DvbSI = new Guid(0xe9dd31a3, 0x221d, 0x4adb, 0x85, 0x32, 0x9a, 0xf3, 0x09, 0xc1, 0xa4, 0x08);

        /// <summary> MEDIASUBTYPE_ATSC_SI </summary>
        public static readonly Guid AtscSI = new Guid(0xb3c7397c, 0xd303, 0x414d, 0xb3, 0x3c, 0x4e, 0xd2, 0xc9, 0xd2, 0x97, 0x33);

        /// <summary> MEDIASUBTYPE_MPEG2DATA </summary>
        public static readonly Guid Mpeg2Data = new Guid(0xc892e55b, 0x252d, 0x42b5, 0xa3, 0x16, 0xd9, 0x97, 0xe7, 0xa5, 0xd9, 0x95);

        /// <summary> MEDIASUBTYPE_MPEG2_PROGRAM </summary>
        public static readonly Guid Mpeg2Program = new Guid(0xe06d8022, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_MPEG2_TRANSPORT </summary>
        public static readonly Guid Mpeg2Transport = new Guid(0xe06d8023, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_MPEG2_TRANSPORT_STRIDE </summary>
        public static readonly Guid Mpeg2TransportStride = new Guid(0x138aa9a4, 0x1ee2, 0x4c5b, 0x98, 0x8e, 0x19, 0xab, 0xfd, 0xbc, 0x8a, 0x11);

        /// <summary> MEDIASUBTYPE_None </summary>
        public static readonly Guid None = new Guid(0xe436eb8e, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_H264 </summary>
        public static readonly Guid H264 = new Guid(0x34363248, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_NV24 </summary>
        public static readonly Guid NV24 = new Guid(0x3432564E, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_708_608Data </summary>
        public static readonly Guid Data708_608 = new Guid(0xaf414bc, 0x4ed2, 0x445e, 0x98, 0x39, 0x8f, 0x9, 0x55, 0x68, 0xab, 0x3c);

        /// <summary> MEDIASUBTYPE_DtvCcData </summary>
        public static readonly Guid DtvCcData = new Guid(0xF52ADDAA, 0x36F0, 0x43F5, 0x95, 0xEA, 0x6D, 0x86, 0x64, 0x84, 0x26, 0x2A);

        /// <summary> MEDIASUBTYPE_DVB_SUBTITLES </summary>
        public static readonly Guid DVB_Subtitles = new Guid(0x34FFCBC3, 0xD5B3, 0x4171, 0x90, 0x02, 0xD4, 0xC6, 0x03, 0x01, 0x69, 0x7F);

        /// <summary> MEDIASUBTYPE_ISDB_CAPTIONS </summary>
        public static readonly Guid ISDB_Captions = new Guid(0x059dd67d, 0x2e55, 0x4d41, 0x8d, 0x1b, 0x01, 0xf5, 0xe4, 0xf5, 0x06, 0x07);

        /// <summary> MEDIASUBTYPE_ISDB_SUPERIMPOSE </summary>
        public static readonly Guid ISDB_Superimpose = new Guid(0x36dc6d28, 0xf1a6, 0x4216, 0x90, 0x48, 0x9c, 0xfc, 0xef, 0xeb, 0x5e, 0xba);

        /// <summary> MEDIASUBTYPE_NV11 </summary>
        public static readonly Guid NV11 = new Guid(0x3131564E, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P208 </summary>
        public static readonly Guid P208 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P210 </summary>
        public static readonly Guid P210 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P216 </summary>
        public static readonly Guid P216 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P010 </summary>
        public static readonly Guid P010 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P016 </summary>
        public static readonly Guid P016 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y210 </summary>
        public static readonly Guid Y210 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y216 </summary>
        public static readonly Guid Y216 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P408 </summary>
        public static readonly Guid P408 = new Guid(0x38303450, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CC_CONTAINER </summary>
        public static readonly Guid CC_Container = new Guid(0x7ea626db, 0x54da, 0x437b, 0xbe, 0x9f, 0xf7, 0x30, 0x73, 0xad, 0xfa, 0x3c);

        /// <summary> MEDIASUBTYPE_VBI </summary>
        public static readonly Guid VBI = new Guid(0x663da43c, 0x3e8, 0x4e9a, 0x9c, 0xd5, 0xbf, 0x11, 0xed, 0xd, 0xef, 0x76);

        /// <summary> MEDIASUBTYPE_XDS </summary>
        public static readonly Guid XDS = new Guid(0x1ca73e3, 0xdce6, 0x4575, 0xaf, 0xe1, 0x2b, 0xf1, 0xc9, 0x2, 0xca, 0xf3);

        /// <summary> MEDIASUBTYPE_ETDTFilter_Tagged </summary>
        public static readonly Guid ETDTFilter_Tagged = new Guid(0xC4C4C4D0, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> MEDIASUBTYPE_CPFilters_Processed </summary>
        public static readonly Guid CPFilters_Processed = new Guid(0x46adbd28, 0x6fd0, 0x4796, 0x93, 0xb2, 0x15, 0x5c, 0x51, 0xdc, 0x4, 0x8d);

    }

    static public class FormatType
    {
        public static readonly Guid Null = Guid.Empty;

        /// <summary> FORMAT_None </summary>
        public static readonly Guid None = new Guid(0x0F6417D6, 0xc318, 0x11d0, 0xa4, 0x3f, 0x00, 0xa0, 0xc9, 0x22, 0x31, 0x96);

        /// <summary> FORMAT_VideoInfo </summary>
        public static readonly Guid VideoInfo = new Guid(0x05589f80, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_VideoInfo2 </summary>
        public static readonly Guid VideoInfo2 = new Guid(0xf72a76A0, 0xeb0a, 0x11d0, 0xac, 0xe4, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> FORMAT_WaveFormatEx </summary>
        public static readonly Guid WaveEx = new Guid(0x05589f81, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_MPEGVideo </summary>
        public static readonly Guid MpegVideo = new Guid(0x05589f82, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_MPEGStreams </summary>
        public static readonly Guid MpegStreams = new Guid(0x05589f83, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_DvInfo </summary>
        public static readonly Guid DvInfo = new Guid(0x05589f84, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_AnalogVideo </summary>
        public static readonly Guid AnalogVideo = new Guid(0x0482dde0, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> FORMAT_MPEG2Video </summary>
        public static readonly Guid Mpeg2Video = new Guid(0xe06d80e3, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> FORMAT_DolbyAC3 </summary>
        public static readonly Guid DolbyAC3 = new Guid(0xe06d80e4, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> FORMAT_MPEG2Audio </summary>
        public static readonly Guid Mpeg2Audio = new Guid(0xe06d80e5, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> FORMAT_525WSS </summary>
        public static readonly Guid WSS525 = new Guid(0xc7ecf04d, 0x4582, 0x4869, 0x9a, 0xbb, 0xbf, 0xb5, 0x23, 0xb6, 0x2e, 0xdf);

        /// <summary> FORMATTYPE_ETDTFilter_Tagged </summary>
        public static readonly Guid ETDTFilter_Tagged = new Guid(0xC4C4C4D1, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> FORMATTYPE_CPFilters_Processed </summary>
        public static readonly Guid CPFilters_Processed = new Guid(0x6739b36f, 0x1d5f, 0x4ac2, 0x81, 0x92, 0x28, 0xbb, 0xe, 0x73, 0xd1, 0x6a);
    }

    static public class DSAttrib
    {
        /// <summary> DSATTRIB_UDCRTag </summary>
        public static readonly Guid UDCRTag = new Guid(0xEB7836CA, 0x14FF, 0x4919, 0xbc, 0xe7, 0x3a, 0xf1, 0x23, 0x19, 0xe5, 0x0c);

        /// <summary> DSATTRIB_PicSampleSeq </summary>
        public static readonly Guid PicSampleSeq = new Guid(0x2f5bae02, 0x7b8f, 0x4f60, 0x82, 0xd6, 0xe4, 0xea, 0x2f, 0x1f, 0x4c, 0x99);

        /// <summary> DSATTRIB_OptionalVideoAttributes </summary>
        public static readonly Guid OptionalVideoAttributes = new Guid(0x5A5F08CA, 0x55C2, 0x4033, 0x92, 0xAB, 0x55, 0xDB, 0x8F, 0x78, 0x12, 0x26);

        /// <summary> DSATTRIB_CC_CONTAINER_INFO </summary>
        public static readonly Guid CC_ContainerInfo = new Guid(0xe7e050fb, 0xdd5d, 0x40dd, 0x99, 0x15, 0x35, 0xDC, 0xB8, 0x1B, 0xDC, 0x8a);

        /// <summary> DSATTRIB_TRANSPORT_PROPERTIES </summary>
        public static readonly Guid TransportProperties = new Guid(0xb622f612, 0x47ad, 0x4671, 0xad, 0x6c, 0x5, 0xa9, 0x8e, 0x65, 0xde, 0x3a);

        /// <summary> DSATTRIB_PBDATAG_ATTRIBUTE </summary>
        public static readonly Guid PBDATagAttribute = new Guid(0xe0b56679, 0x12b9, 0x43cc, 0xb7, 0xdf, 0x57, 0x8c, 0xaa, 0x5a, 0x7b, 0x63);

        /// <summary> DSATTRIB_CAPTURE_STREAMTIME </summary>
        public static readonly Guid CaptureStreamtime = new Guid(0x0c1a5614, 0x30cd, 0x4f40, 0xbc, 0xbf, 0xd0, 0x3e, 0x52, 0x30, 0x62, 0x07);

        /// <summary> DSATTRIB_DSHOW_STREAM_DESC </summary>
        public static readonly Guid DShowStreamDesc = new Guid(0x5fb5673b, 0xa2a, 0x4565, 0x82, 0x7b, 0x68, 0x53, 0xfd, 0x75, 0xe6, 0x11);

        /// <summary> DSATTRIB_SAMPLE_LIVE_STREAM_TIME </summary>
        public static readonly Guid SampleLiveStreamtime = new Guid(0x892cd111, 0x72f3, 0x411d, 0x8b, 0x91, 0xa9, 0xe9, 0x12, 0x3a, 0xc2, 0x9a);

        /// <summary> DSATTRIB_WMDRMProtectionInfo </summary>
        public static readonly Guid WMDRMProtectionInfo = new Guid(0x40749583, 0x6b9d, 0x4eec, 0xb4, 0x3c, 0x67, 0xa1, 0x80, 0x1e, 0x1a, 0x9b );

        /// <summary> DSATTRIB_BadSampleInfo </summary>
        public static readonly Guid BadSampleInfo = new Guid(0xe4846dda, 0x5838, 0x42b4, 0xb8, 0x97, 0x6f, 0x7e, 0x5f, 0xaa, 0x2f, 0x2f);

    }

    static public class PropSetID
    {
        /// <summary> AMPROPSETID_Pin</summary>
        public static readonly Guid Pin = new Guid(0x9b00f101, 0x1567, 0x11d1, 0xb3, 0xf1, 0x00, 0xaa, 0x00, 0x37, 0x61, 0xc5);

        /// <summary> PROPSETID_VIDCAP_DROPPEDFRAMES </summary>
        public static readonly Guid DroppedFrames = new Guid(0xC6E13344, 0x30AC, 0x11D0, 0xA1, 0x8C, 0x00, 0xA0, 0xC9, 0x11, 0x89, 0x56);

        /// <summary> STATIC_ENCAPIPARAM_BITRATE </summary>
        public static readonly Guid ENCAPIPARAM_BitRate = new Guid(0x49cc4c43, 0xca83, 0x4ad4, 0xa9, 0xaf, 0xf3, 0x69, 0x6a, 0xf6, 0x66, 0xdf);

        /// <summary> STATIC_ENCAPIPARAM_PEAK_BITRATE </summary>
        public static readonly Guid ENCAPIPARAM_PeakBitRate = new Guid(0x703f16a9, 0x3d48, 0x44a1, 0xb0, 0x77, 0x01, 0x8d, 0xff, 0x91, 0x5d, 0x19);

        /// <summary> STATIC_ENCAPIPARAM_BITRATE_MODE </summary>
        public static readonly Guid ENCAPIPARAM_BitRateMode = new Guid(0xee5fb25c, 0xc713, 0x40d1, 0x9d, 0x58, 0xc0, 0xd7, 0x24, 0x1e, 0x25, 0x0f);

        /// <summary> ENCAPIPARAM_SAP_MODE </summary>
        public static readonly Guid ENCAPIPARAM_SAP_MODE = new Guid(0xc0171db, 0xfefc, 0x4af7, 0x99, 0x91, 0xa5, 0x65, 0x7c, 0x19, 0x1c, 0xd1);

        /// <summary> CODECAPI_AVDecMmcssClass </summary>
        public static readonly Guid CODECAPI_AVDecMmcssClass = new Guid(0xe0ad4828, 0xdf66, 0x4893, 0x9f, 0x33, 0x78, 0x8a, 0xa4, 0xec, 0x40, 0x82);

        /// <summary> STATIC_CODECAPI_CHANGELISTS </summary>
        public static readonly Guid CODECAPI_ChangeLists = new Guid(0x62b12acf, 0xf6b0, 0x47d9, 0x94, 0x56, 0x96, 0xf2, 0x2c, 0x4e, 0x0b, 0x9d);

        /// <summary> STATIC_CODECAPI_VIDEO_ENCODER </summary>
        public static readonly Guid CODECAPI_VideoEncoder = new Guid(0x7112e8e1, 0x3d03, 0x47ef, 0x8e, 0x60, 0x03, 0xf1, 0xcf, 0x53, 0x73, 0x01);

        /// <summary> STATIC_CODECAPI_AUDIO_ENCODER </summary>
        public static readonly Guid CODECAPI_AudioEncoder = new Guid(0xb9d19a3e, 0xf897, 0x429c, 0xbc, 0x46, 0x81, 0x38, 0xb7, 0x27, 0x2b, 0x2d);

        /// <summary> STATIC_CODECAPI_SETALLDEFAULTS </summary>
        public static readonly Guid CODECAPI_SetAllDefaults = new Guid(0x6c5e6a7c, 0xacf8, 0x4f55, 0xa9, 0x99, 0x1a, 0x62, 0x81, 0x09, 0x05, 0x1b);

        /// <summary> STATIC_CODECAPI_ALLSETTINGS </summary>
        public static readonly Guid CODECAPI_AllSettings = new Guid(0x6a577e92, 0x83e1, 0x4113, 0xad, 0xc2, 0x4f, 0xce, 0xc3, 0x2f, 0x83, 0xa1);

        /// <summary> STATIC_CODECAPI_SUPPORTSEVENTS </summary>
        public static readonly Guid CODECAPI_SupportsEvents = new Guid(0x0581af97, 0x7693, 0x4dbd, 0x9d, 0xca, 0x3f, 0x9e, 0xbd, 0x65, 0x85, 0xa1);

        /// <summary> STATIC_CODECAPI_CURRENTCHANGELIST </summary>
        public static readonly Guid CODECAPI_CurrentChangeList = new Guid(0x1cb14e83, 0x7d72, 0x4657, 0x83, 0xfd, 0x47, 0xa2, 0xc5, 0xb9, 0xd1, 0x3d);

    }

    static public class PinCategory
    {
        /// <summary> PIN_CATEGORY_CAPTURE </summary>
        public static readonly Guid Capture = new Guid(0xfb6c4281, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_PREVIEW </summary>
        public static readonly Guid Preview = new Guid(0xfb6c4282, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_ANALOGVIDEOIN </summary>
        public static readonly Guid AnalogVideoIn = new Guid(0xfb6c4283, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_VBI </summary>
        public static readonly Guid VBI = new Guid(0xfb6c4284, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_VIDEOPORT </summary>
        public static readonly Guid VideoPort = new Guid(0xfb6c4285, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_NABTS </summary>
        public static readonly Guid NABTS = new Guid(0xfb6c4286, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_EDS </summary>
        public static readonly Guid EDS = new Guid(0xfb6c4287, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_TELETEXT </summary>
        public static readonly Guid TeleText = new Guid(0xfb6c4288, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_CC </summary>
        public static readonly Guid CC = new Guid(0xfb6c4289, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_STILL </summary>
        public static readonly Guid Still = new Guid(0xfb6c428a, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_TIMECODE </summary>
        public static readonly Guid TimeCode = new Guid(0xfb6c428b, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_VIDEOPORT_VBI </summary>
        public static readonly Guid VideoPortVBI = new Guid(0xfb6c428c, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

    }

    static public class FindDirection
    {
        /// <summary> LOOK_UPSTREAM_ONLY </summary>
        public static readonly Guid UpstreamOnly = new Guid(0xac798be0, 0x98e3, 0x11d1, 0xb3, 0xf1, 0x00, 0xaa, 0x00, 0x37, 0x61, 0xc5);

        /// <summary> LOOK_DOWNSTREAM_ONLY </summary>
        public static readonly Guid DownstreamOnly = new Guid(0xac798be1, 0x98e3, 0x11d1, 0xb3, 0xf1, 0x00, 0xaa, 0x00, 0x37, 0x61, 0xc5);
    }

    static public class TimeFormat
    {
        // 00000000-0000-0000-0000-000000000000 TIME_FORMAT_NONE
        public static readonly Guid None = new Guid(0x0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        // 7b785570-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_FRAME
        public static readonly Guid Frame = new Guid(0x7b785570, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785571-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_BYTE
        public static readonly Guid Byte = new Guid(0x7b785571, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785572-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_SAMPLE
        public static readonly Guid Sample = new Guid(0x7b785572, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785573-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_FIELD
        public static readonly Guid Field = new Guid(0x7b785573, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785574-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_MEDIA_TIME
        public static readonly Guid MediaTime = new Guid(0x7b785574, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);
    }

    static public class PropertyPages
    {
        /// <summary> CLSID_CrossbarFilterPropertyPage </summary>
        public static readonly Guid CrossbarFilterPropertyPage = new Guid(0x71f96461, 0x78f3, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0xc9, 0x11, 0x89, 0x56);

        /// <summary> CLSID_AudioInputMixerProperties </summary>
        public static readonly Guid AudioInputMixer = new Guid(0x2ca8ca52, 0x3c3f, 0x11d2, 0xb7, 0x3d, 0x00, 0xc0, 0x4f, 0xb6, 0xbd, 0x3d);

        /// <summary> CLSID_AudioProperties </summary>
        public static readonly Guid AudioProperties = new Guid(0x05589faf, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> CLSID_AudioRendererAdvancedProperties </summary>
        public static readonly Guid AudioRendererAdvancedProperties = new Guid(0x37e92a92, 0xd9aa, 0x11d2, 0xbf, 0x84, 0x8e, 0xf2, 0xb1, 0x55, 0x5a, 0xed);

        /// <summary> CLSID_AviMuxProptyPage </summary>
        public static readonly Guid AviMux = new Guid(0xc647b5c0, 0x157c, 0x11d0, 0xbd, 0x23, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> CLSID_AviMuxProptyPage1 </summary>
        public static readonly Guid AviMux1 = new Guid(0x0a9ae910, 0x85c0, 0x11d0, 0xbd, 0x42, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> CLSID_DVDecPropertiesPage </summary>
        public static readonly Guid DVDec = new Guid(0x101193c0, 0x0bfe, 0x11d0, 0xaf, 0x91, 0x00, 0xaa, 0x00, 0xb6, 0x7a, 0x42);

        /// <summary> CLSID_DVEncPropertiesPage </summary>
        public static readonly Guid DVEnc = new Guid(0x4150f050, 0xbb6f, 0x11d0, 0xaf, 0xb9, 0x00, 0xaa, 0x00, 0xb6, 0x7a, 0x42);

        /// <summary> CLSID_ModexProperties </summary>
        public static readonly Guid Modex = new Guid(0x0618aa30, 0x6bc4, 0x11cf, 0xbf, 0x36, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> CLSID_CaptureProperties </summary>
        public static readonly Guid Capture = new Guid(0x1B544c22, 0xFD0B, 0x11ce, 0x8C, 0x63, 0x00, 0xAA, 0x00, 0x44, 0xB5, 0x1F);

        /// <summary> CLSID_WstDecoderPropertyPage </summary>
        public static readonly Guid WstDecoder = new Guid(0x04e27f80, 0x91e4, 0x11d3, 0xa1, 0x84, 0x00, 0x10, 0x5a, 0xef, 0x9f, 0x33);

        /// <summary> CLSID_DVMuxPropertyPage </summary>
        public static readonly Guid DVMux = new Guid(0x4db880e0, 0xc10d, 0x11d0, 0xaf, 0xb9, 0x00, 0xaa, 0x00, 0xb6, 0x7a, 0x42);

        /// <summary> CLSID_VideoProcAmpPropertyPage </summary>
        public static readonly Guid VideoProcAmpPropertyPage = new Guid(0x71f96464, 0x78f3, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0xc9, 0x11, 0x89, 0x56);

        /// <summary> CLSID_CameraControlPropertyPage </summary>
        public static readonly Guid CameraControlPropertyPage = new Guid(0x71f96465, 0x78f3, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0xc9, 0x11, 0x89, 0x56);

        /// <summary> CLSID_AnalogVideoDecoderPropertyPage </summary>
        public static readonly Guid AnalogVideoDecoderPropertyPage = new Guid(0x71f96466, 0x78f3, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0xc9, 0x11, 0x89, 0x56);

        /// <summary> CLSID_VideoStreamConfigPropertyPage </summary>
        public static readonly Guid VideoStreamConfigPropertyPage = new Guid(0x71f96467, 0x78f3, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0xc9, 0x11, 0x89, 0x56);

        /// <summary> CLSID_ATSCNetworkPropertyPage </summary>
        public static readonly Guid ATSCNetworkPropertyPage = new Guid(0xe3444d16, 0x5ac4, 0x4386, 0x88, 0xdf, 0x13, 0xfd, 0x23, 0x0e, 0x1d, 0xda);

        /// <summary> CLSID_TVTunerFilterPropertyPage </summary>
        public static readonly Guid TVTunerFilterPropertyPage = new Guid(0x266eee41, 0x6c63, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> CLSID_TVAudioFilterPropertyPage </summary>
        public static readonly Guid TVAudioFilterPropertyPage = new Guid(0x71f96463, 0x78f3, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0xc9, 0x11, 0x89, 0x56);
    }

    static public class BDANodeCategory
    {
        /// <summary> KSNODE_BDA_RF_TUNER </summary>
        public static readonly Guid RFTuner = new Guid(0x71985f4c, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSNODE_BDA_QAM_DEMODULATOR </summary>
        public static readonly Guid QAMDemodulator = new Guid(0x71985f4d, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSNODE_BDA_QPSK_DEMODULATOR </summary>
        public static readonly Guid QPSKDemodulator = new Guid(0x6390c905, 0x27c1, 0x4d67, 0xbd, 0xb7, 0x77, 0xc5, 0x0d, 0x07, 0x93, 0x00);

        /// <summary> KSNODE_BDA_8VSB_DEMODULATOR </summary>
        public static readonly Guid EightVSBDemodulator = new Guid(0x71985f4f, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSNODE_BDA_COFDM_DEMODULATOR </summary>
        public static readonly Guid COFDMDemodulator = new Guid(0x2dac6e05, 0xedbe, 0x4b9c, 0xb3, 0x87, 0x1b, 0x6f, 0xad, 0x7d, 0x64, 0x95);

        /// <summary> KSNODE_BDA_OPENCABLE_POD </summary>
        public static readonly Guid OpenCablePod = new Guid(0x345812a0, 0xfb7c, 0x4790, 0xaa, 0x7e, 0xb1, 0xdb, 0x88, 0xac, 0x19, 0xc9);

        /// <summary> KSNODE_BDA_COMMON_CA_POD </summary>
        public static readonly Guid CommonCAPod = new Guid(0xd83ef8fc, 0xf3b8, 0x45ab, 0x8b, 0x71, 0xec, 0xf7, 0xc3, 0x39, 0xde, 0xb4);

        /// <summary> KSNODE_BDA_PID_FILTER </summary>
        public static readonly Guid PidFilter = new Guid(0xf5412789, 0xb0a0, 0x44e1, 0xae, 0x4f, 0xee, 0x99, 0x9b, 0x1b, 0x7f, 0xbe);

        /// <summary> KSNODE_BDA_IP_SINK </summary>
        public static readonly Guid IPSink = new Guid(0x71985f4e, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

    }

    static public class TAGTables
    {
        /// <summary> UUID_UdriTagTables </summary>
        public static readonly Guid UdriTagTables = new Guid(0xe1b98d74, 0x9778, 0x4878, 0xb6, 0x64, 0xeb, 0x20, 0x20, 0x36, 0x4d, 0x88);

        /// <summary> UUID_WMDRMTagTables </summary>
        public static readonly Guid WMDRMTagTables = new Guid(0x5DCD1101, 0x9263, 0x45bb, 0xa4, 0xd5, 0xc4, 0x15, 0xab, 0x8c, 0x58, 0x9c);
    }

    #endregion
}
