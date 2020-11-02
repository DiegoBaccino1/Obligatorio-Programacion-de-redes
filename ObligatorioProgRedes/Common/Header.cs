using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Header
    {
        private byte[] _direction;
        private byte[] _command;
        private byte[] _dataLength;


        private string _sDirection;
        public void SetDirection(string direction)
        {
            this._sDirection = direction;
        }
        public string GetDirection()
        {
            return this._sDirection;
        }
        
        private int _iCommand;
        public void SetCommand(int command)
        {
            this._iCommand = command;
        }
        public int GetCommand()
        {
            return this._iCommand;
        }
        
        private int _iDataLength;
        public void SetDataLength(int datalength)
        {
            this._iDataLength = datalength;
        }
        public int GetDataLength()
        {
            return this._iDataLength;
        }

        public Header(string direction, int command, int dataLenght)
        {
            _direction = Encoding.UTF8.GetBytes(direction);
            var commandToString = command.ToString("D2");
            _command = Encoding.UTF8.GetBytes(commandToString);
            var dataLengthToString = dataLenght.ToString("D5");
            _dataLength = Encoding.UTF8.GetBytes(dataLengthToString);
        }
        public Header(byte[] header)
        {
            Decode(header);
        }
        public byte[] GenRequest()
        {
            var header = new byte[HeaderConstants.Request.Length + HeaderConstants.CommandLength + HeaderConstants.DataLength];
            Array.Copy(_direction, 0, header, 0,HeaderConstants.Request.Length);

            Array.Copy(_command, 0, header, HeaderConstants.Request.Length,HeaderConstants.CommandLength);

            Array.Copy(_dataLength, 0, header, HeaderConstants.CommandLength +
                            HeaderConstants.Request.Length, HeaderConstants.DataLength);
            return header;
        }

        public bool Decode(byte[] _recivedData)
        {
            try
            {
                _sDirection = DecodeDirection(_recivedData);
                _iCommand = DecodeCommand(_recivedData);
                _iDataLength = DecodeDataLength(_recivedData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string DecodeDirection(byte[] _recivedData)
        {
            return Encoding.UTF8.GetString(_recivedData, 0, HeaderConstants.Request.Length);
        }

        public static int DecodeCommand(byte[] _recivedData)
        {
            return Int32.Parse(Encoding.UTF8.GetString
                (_recivedData, HeaderConstants.Request.Length, HeaderConstants.CommandLength));
        }
        public static int DecodeDataLength(byte[] _recivedData)
        {
            return Int32.Parse(Encoding.UTF8.GetString(_recivedData,
                    HeaderConstants.Request.Length + HeaderConstants.CommandLength, HeaderConstants.DataLength));
        }
    }
}
