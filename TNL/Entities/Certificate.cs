namespace TNL.Entities
{
    using Utils;

    public class Certificate : ByteBuffer
    {
        public const uint MaxPayloadSize = 512;

        protected AsymmetricKey PublicKey { get; set; }
        protected ByteBuffer PayLoad { get; set; }
        protected ByteBuffer Signature { get; set; }
        protected bool PIsValid { get; set; }
        protected uint SignatureByteSize { get; set; }

        public Certificate(byte[] data, uint dataSize)
            : base(data, dataSize)
        {
            SignatureByteSize = 0;
            PIsValid = false;

            Parse();
        }

        public Certificate(BitStream stream)
        {
            SignatureByteSize = 0;
            PIsValid = false;

            stream.Read(this);

            Parse();
        }

        public Certificate(ByteBuffer payload, AsymmetricKey publicKey, AsymmetricKey theCAPrivateKey)
        {
            PIsValid = false;
            SignatureByteSize = 0;

            if (payload.GetBufferSize() > MaxPayloadSize || !publicKey.IsValid())
                return;

            var thePublicKey = PublicKey.GetPublicKey();
            var packet = new PacketStream();

            packet.Write(payload);
            packet.Write(thePublicKey);

            SignatureByteSize = packet.GetBytePosition();
            packet.SetBytePosition(SignatureByteSize);

            var theSignedBytes = new ByteBuffer(packet.GetBuffer(), packet.GetBytePosition());

            Signature = theCAPrivateKey.HashAndSign(theSignedBytes);
            packet.Write(Signature);

            SetBuffer(packet.GetBuffer(), packet.GetBytePosition());
        }

        public void Parse()
        {
            var aStream = new BitStream(GetBuffer(), GetBufferSize());

            PayLoad = new ByteBuffer(0U);
            aStream.Read(PayLoad);

            PublicKey = new AsymmetricKey(aStream);
            Signature = new ByteBuffer(0U);

            SignatureByteSize = aStream.GetBytePosition();

            aStream.SetBytePosition(SignatureByteSize);

            aStream.Read(Signature);

            if (aStream.IsValid() && GetBufferSize() == aStream.GetBytePosition() && PublicKey.IsValid())
                PIsValid = true;
        }

        public bool IsValid()
        {
            return PIsValid;
        }

        public bool Validate(AsymmetricKey signatoryPublicKey)
        {
            return PIsValid && signatoryPublicKey.VerifySignature(new ByteBuffer(GetBuffer(), SignatureByteSize), Signature);
        }

        public AsymmetricKey GetPublicKey()
        {
            return PublicKey;
        }

        public ByteBuffer GetPayload()
        {
            return PayLoad;
        }
    }
}
