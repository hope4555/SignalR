// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Sockets.Formatters
{
    public static class MessageFormatter
    {
        public static readonly byte TextFormatIndicator = (byte)'T';
        public static readonly byte BinaryFormatIndicator = (byte)'B';

        public static readonly string TextContentType = "application/vnd.microsoft.aspnet.endpoint-messages.v1+text";
        public static readonly string BinaryContentType = "application/vnd.microsoft.aspnet.endpoint-messages.v1+binary";

        public static bool TryFormatMessage(Message message, Span<byte> buffer, MessageFormat format, out int bytesWritten)
        {
            if (!message.EndOfMessage)
            {
                // This is a truely exceptional condition since we EXPECT callers to have already
                // buffered incomplete messages and synthesized the correct, complete message before
                // giving it to us. Hence we throw, instead of returning false.
                throw new InvalidOperationException("Cannot format message where endOfMessage is false using this format");
            }

            return format == MessageFormat.Text ?
                TextMessageFormatter.TryFormatMessage(message, buffer, out bytesWritten) :
                BinaryMessageFormatter.TryFormatMessage(message, buffer, out bytesWritten);
        }

        public static bool TryParseMessage(ReadOnlySpan<byte> buffer, MessageFormat format, out Message message, out int bytesConsumed)
        {
            return format == MessageFormat.Text ?
                TextMessageFormatter.TryParseMessage(buffer, out message, out bytesConsumed) :
                BinaryMessageFormatter.TryParseMessage(buffer, out message, out bytesConsumed);
        }

        public static string GetContentType(MessageFormat messageFormat)
        {
            switch (messageFormat)
            {
                case MessageFormat.Text: return TextContentType;
                case MessageFormat.Binary: return BinaryContentType;
                default: throw new InvalidOperationException($"Invalid message format: {messageFormat}");
            }
        }

        public static byte GetFormatIndicator(MessageFormat messageFormat)
        {
            switch (messageFormat)
            {
                case MessageFormat.Text: return TextFormatIndicator;
                case MessageFormat.Binary: return BinaryFormatIndicator;
                default: throw new InvalidOperationException($"Invalid message format: {messageFormat}");
            }
        }
    }
}
