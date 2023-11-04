﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace NodeNet.NodeNet.RSASigner
{
    internal class RSAEncryption
    {
        // Generate new keys for sender
        public static SenderSignOptions CreateSignOptions()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string publicKey = rsa.ToXmlString(false);
                string privateKey = rsa.ToXmlString(true);
                return new SenderSignOptions(publicKey, privateKey);
            }
        }

        public static String Sign(byte[] data, SenderSignOptions options)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(options.PrivateKey);
            var sign = rsa.SignData(data, new SHA256CryptoServiceProvider());
            return Convert.ToBase64String(sign);
        }

        public static bool VerifySign(byte[] data, string sign, ReceiverSignOptions options)
        {   
            var signBytes = Convert.FromBase64String(sign);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(options.PublicKey);
            return rsa.VerifyData(data, new SHA256CryptoServiceProvider(), signBytes);
        }
    }
}
