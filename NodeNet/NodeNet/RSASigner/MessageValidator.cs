﻿using NodeNet.NodeNet.Message;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using NodeNet.NodeNet.RSAEncryptions;

namespace NodeNet.NodeNet.RSASigner
{
    internal class MessageValidator : IMessageValidator
    {
        public ReceiverSignOptions ValidateOptions { get; protected set; }


        public void SetValidateOptions(IReceiverSignOptions options)
        {
            ValidateOptions = options as ReceiverSignOptions;
            if (ValidateOptions == null)
                throw new ArgumentException(nameof(options));
        }

        public bool Validate(Message.Message message)
        {
            if (ValidateOptions == null)
                throw new NullReferenceException(nameof(ValidateOptions));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, message.Info);
                formatter.Serialize(memoryStream, message.Data);
                return RSAEncryption.VerifySign(memoryStream.ToArray(), message.MessageSign, ValidateOptions);
            }
        }
    }
}
