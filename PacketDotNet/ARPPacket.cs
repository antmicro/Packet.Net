/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using System.Net.NetworkInformation;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An ARP protocol packet.
    /// </summary>
    public class ARPPacket : InternetLinkLayerPacket
    {
        /// <value>
        /// Also known as HardwareType
        /// </value>
        virtual public int HardwareAddressType
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + ARPFields.HardwareAddressTypePosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + ARPFields.HardwareAddressTypePosition);
            }
        }

        /// <value>
        /// Also known as ProtocolType
        /// </value>
        virtual public int ProtocolAddressType
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + ARPFields.ProtocolAddressTypePosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + ARPFields.ProtocolAddressTypePosition);
            }
        }

        /// <value>
        /// Hardware address length field
        /// </value>
        virtual public int HardwareAddressLength
        {
            get
            {
                return header.Bytes[header.Offset + ARPFields.HardwareAddressLengthPosition];
            }

            set
            {
                header.Bytes[header.Offset + ARPFields.HardwareAddressLengthPosition] = (byte)value;
            }
        }

        /// <value>
        /// Protocol address length field
        /// </value>
        virtual public int ProtocolAddressLength
        {
            get
            {
                return header.Bytes[header.Offset + ARPFields.ProtocolAddressLengthPosition];
            }

            set
            {
                header.Bytes[header.Offset + ARPFields.ProtocolAddressLengthPosition] = (byte)value;
            }
        }

        /// <summary> Fetch the operation code.
        /// Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
        /// </summary>
        /// <summary> Sets the operation code.
        /// Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
        /// </summary>
        virtual public ARPOperation Operation
        {
            get
            {
                return (ARPOperation)EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                    header.Offset + ARPFields.OperationPosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + ARPFields.OperationPosition);
            }
        }

        /// <value>
        /// Upper layer protocol address of the sender, typically an IPv4 or IPv6 address
        /// </value>
        virtual public System.Net.IPAddress SenderProtocolAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             header.Offset + ARPFields.SenderProtocolAddressPosition,
                                             header.Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                           header.Bytes, header.Offset + ARPFields.SenderProtocolAddressPosition,
                           address.Length);
            }
        }

        /// <value>
        /// Upper layer protocol address of the target, typically an IPv4 or IPv6 address
        /// </value>
        virtual public System.Net.IPAddress TargetProtocolAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             header.Offset + ARPFields.TargetProtocolAddressPosition,
                                             header.Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                           header.Bytes, header.Offset + ARPFields.TargetProtocolAddressPosition,
                           address.Length);
            }
        }

        /// <value>
        /// Sender hardware address, usually an ethernet mac address
        /// </value>
        public virtual PhysicalAddress SenderHardwareAddress
        {
            get
            {
                //FIXME: this code is broken because it assumes that the address position is
                // a fixed position
                byte[] hwAddress = new byte[HardwareAddressLength];
                Array.Copy(header.Bytes, header.Offset + ARPFields.SenderHardwareAddressPosition,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                byte[] hwAddress = value.GetAddressBytes();

                // for now we only support ethernet addresses even though the arp protocol
                // makes provisions for varying length addresses
                if(hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new System.InvalidOperationException("expected physical address length of "
                                                               + EthernetFields.MacAddressLength
                                                               + " but it was "
                                                               + hwAddress.Length);
                }

                Array.Copy(hwAddress, 0,
                           header.Bytes, header.Offset + ARPFields.SenderHardwareAddressPosition,
                           hwAddress.Length);
            }
        }

        /// <value>
        /// Target hardware address, usually an ethernet mac address
        /// </value>
        public virtual PhysicalAddress TargetHardwareAddress
        {
            get
            {
                //FIXME: this code is broken because it assumes that the address position is
                // a fixed position
                byte[] hwAddress = new byte[HardwareAddressLength];
                Array.Copy(header.Bytes, header.Offset + ARPFields.TargetHardwareAddressPosition,
                           hwAddress, 0,
                           hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }
            set
            {
                byte[] hwAddress = value.GetAddressBytes();

                // for now we only support ethernet addresses even though the arp protocol
                // makes provisions for varying length addresses
                if(hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new System.InvalidOperationException("expected physical address length of "
                                                               + EthernetFields.MacAddressLength
                                                               + " but it was "
                                                               + hwAddress.Length);
                }

                Array.Copy(hwAddress, 0,
                           header.Bytes, header.Offset + ARPFields.TargetHardwareAddressPosition,
                           hwAddress.Length);
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Purple;
            }
        }

        /// <summary>
        /// byte[]/int Offset constructor
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public ARPPacket(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        { }

        /// <summary>
        /// byte[]/int offset/PosixTimeval constructor
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public ARPPacket(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            header = new ByteArrayAndOffset(Bytes, Offset, ARPFields.HeaderLength);

            // NOTE: no need to set the payloadPacketOrData field, arp packets have
            //       no payload
        }

        /// <summary> Convert this ARP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this ARP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("ARPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.Append(": ");
            buffer.Append(Operation);
            buffer.Append(' ');
            buffer.Append(SenderHardwareAddress + " -> " + TargetHardwareAddress);
            buffer.Append(", ");
            buffer.Append(SenderProtocolAddress + " -> " + TargetProtocolAddress);
            //buffer.append(" l=" + header.length + "," + data.length);
            buffer.Append(']');

            // append the base string output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary>
        /// Returns true if Packet is an ARPPacket, ie.
        /// an EthernetPacket that contains an ARPPacket
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public static bool IsType(Packet p)
        {
            // an arp packet is a type of InternetLinkLayerPacket
            if(p is InternetLinkLayerPacket)
            {
                if(p.PayloadPacket is ARPPacket)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the encapsulated ARPPacket of the Packet p or null if
        /// there is no encapsulated packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="ARPPacket"/>
        /// </returns>
        public static ARPPacket GetType(Packet p)
        {
            if(IsType(p))
            {
                return (ARPPacket)p.PayloadPacket;
            }

            return null;
        }
    }
}
