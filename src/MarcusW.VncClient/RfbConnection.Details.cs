using System.Collections.Immutable;
using MarcusW.VncClient.Protocol;
using MarcusW.VncClient.Protocol.EncodingTypes;
using MarcusW.VncClient.Protocol.MessageTypes;
using MarcusW.VncClient.Protocol.SecurityTypes;
using MarcusW.VncClient.Rendering;

namespace MarcusW.VncClient
{
    public partial class RfbConnection
    {
        // The properties in this class serve only the purpose to inform the API consumer about some connection details.
        // Classes that are part of the protocol implementation should have their own fields/properties somewhere (e.g. in the connection-context)
        // and not have to use these properties to avoid unnecessary locking.

        private readonly object _protocolVersionLock = new object();
        private RfbProtocolVersion _protocolVersion = RfbProtocolVersion.Unknown;

        private readonly object _usedSecurityTypeLock = new object();
        private ISecurityType? _usedSecurityType;

        private readonly object _usedMessageTypesLock = new object();
        private IImmutableSet<IMessageType> _usedMessageTypes = ImmutableHashSet<IMessageType>.Empty;

        private readonly object _usedEncodingTypesLock = new object();
        private IImmutableSet<IEncodingType> _usedEncodingTypes = ImmutableHashSet<IEncodingType>.Empty;

        private readonly object _remoteFramebufferSizeLock = new object();
        private Size _remoteFramebufferSize = Size.Zero;

        private readonly object _remoteFramebufferFormatLock = new object();
        private PixelFormat _remoteFramebufferFormat = PixelFormat.Unknown;

        private readonly object _desktopNameLock = new object();
        private string? _desktopName;

        /// <summary>
        /// Gets the version of the protocol used for remote communication.
        /// Subscribe to <see cref="PropertyChanged"/> to receive change notifications.
        /// </summary>
        public RfbProtocolVersion ProtocolVersion
        {
            get => GetWithLock(ref _protocolVersion, _protocolVersionLock);
            internal set => RaiseAndSetIfChangedWithLockAndDisposedCheck(ref _protocolVersion, value, _protocolVersionLock);
        }

        /// <summary>
        /// Gets the security type that was used for authenticating and securing the connection.
        /// Subscribe to <see cref="PropertyChanged"/> to receive change notifications.
        /// </summary>
        public ISecurityType? UsedSecurityType
        {
            get => GetWithLock(ref _usedSecurityType, _usedSecurityTypeLock);
            internal set => RaiseAndSetIfChangedWithLockAndDisposedCheck(ref _usedSecurityType, value, _usedSecurityTypeLock);
        }

        /// <summary>
        /// Gets the message types that are currently used by this connection.
        /// Subscribe to <see cref="PropertyChanged"/> to receive change notifications.
        /// </summary>
        public IImmutableSet<IMessageType> UsedMessageTypes
        {
            get => GetWithLock(ref _usedMessageTypes, _usedMessageTypesLock);
            internal set => RaiseAndSetIfChangedWithLockAndDisposedCheck(ref _usedMessageTypes, value, _usedMessageTypesLock);
        }

        /// <summary>
        /// Gets the encoding types that are currently used by this connection.
        /// Subscribe to <see cref="PropertyChanged"/> to receive change notifications.
        /// </summary>
        public IImmutableSet<IEncodingType> UsedEncodingTypes
        {
            get => GetWithLock(ref _usedEncodingTypes, _usedEncodingTypesLock);
            internal set => RaiseAndSetIfChangedWithLockAndDisposedCheck(ref _usedEncodingTypes, value, _usedEncodingTypesLock);
        }

        /// <summary>
        /// Gets the current size of the remote view.
        /// Subscribe to <see cref="PropertyChanged"/> to receive change notifications.
        /// </summary>
        public Size RemoteFramebufferSize
        {
            get => GetWithLock(ref _remoteFramebufferSize, _remoteFramebufferSizeLock);
            internal set => RaiseAndSetIfChangedWithLockAndDisposedCheck(ref _remoteFramebufferSize, value, _remoteFramebufferSizeLock);
        }

        /// <summary>
        /// Gets the current format of the remote view.
        /// Subscribe to <see cref="PropertyChanged"/> to receive change notifications.
        /// </summary>
        public PixelFormat RemoteFramebufferFormat
        {
            get => GetWithLock(ref _remoteFramebufferFormat, _remoteFramebufferFormatLock);
            internal set => RaiseAndSetIfChangedWithLockAndDisposedCheck(ref _remoteFramebufferFormat, value, _remoteFramebufferFormatLock);
        }

        /// <summary>
        /// Gets the current name of the remote desktop.
        /// Subscribe to <see cref="PropertyChanged"/> to receive change notifications.
        /// </summary>
        public string? DesktopName
        {
            get => GetWithLock(ref _desktopName, _desktopNameLock);
            internal set => RaiseAndSetIfChangedWithLockAndDisposedCheck(ref _desktopName, value, _desktopNameLock);
        }
    }
}
