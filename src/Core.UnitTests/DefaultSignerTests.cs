using Draco.Core.Services;
using FluentAssertions;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.UnitTests
{
    public class DefaultSignerTests
    {
        private const string DefaultRsaKeyXml =
            @"<RSAKeyValue>
                <Modulus>r/WV+SzUls2Imn6JLS1h/hZpz9+Ei7bWcWXI136gWeeUjedYAea/dncsgA9BA1Zga9z4B/x9Q/AsqY2ZqtA9ovyTCObyokbOnHflazPYo6oiwwGPZHD9DB92HncYNdGuD4oIGXYbymK+7QGR2odq5wgPjn2sp+BR1idpNa5Yci0=</Modulus>
                <Exponent>AQAB</Exponent>
                <P>2R4dd6E4aGg6NQnnwcKA0fPBoKvbBESfTRl9w8KKf0tzyJPd3Fml0KNwKnC2kD2S4/UmSJWrlsNN0pyNM5Xivw==</P>
                <Q>z3iLKsaeYScLJidRALBcHPI/2d5Lf7YzKdBwFk6H+TjDdad5acUAwfvUpEgfaPVwj2LAJe+hIlj/nZcoFvTiEw==</Q>
                <DP>sDQLkCHsSHeF/bNrPcmfiER9+OpgFeenLQgqE+xXQBW5AqhWXtT2iAuMJTVSRF9kTdKP3kpxdipMh0d8JMhYuw==</DP>
                <DQ>zltGiLBvXZB5DWTzs6y2oe/txGTPUWLG9uMkaXuI0UE9YGvIXoSdIVS7Bv/ZhgsB/wwotFLpQYGmFXgAXv0JUw==</DQ>
                <InverseQ>ZhgSXqzsewf/S+oiOGYkZUlbchrqAMzWRbGHQYRua1biA9gfMqD5klQxriPJWyD0T/3fpNFa9pwQshx7rb2TRQ==</InverseQ>
                <D>dYqlg41Z+d8UyLeMZTxywGwyhOU4QqiBucCXmLSC42vahayr0qFU9wsHFAickmEqCgPTQGp/N1oCS8VGiJ+Q++sBpUFEQkjJfiSzjxrHNSkwncrXSpW/rcqU04iiEw/l7xI7luC9xiWqcA5/gBaM6R8YTFDLJo4bqddRCxIgfBU=</D>
            </RSAKeyValue>";

        [Fact]
        public void GenerateSignatureAsync_GivenNoRsaKeyXml_ShouldThrowArgumentNullException()
        {
            var defaultSignerUt = new DefaultSigner();

            Func<Task> act = async () => await defaultSignerUt.GenerateSignatureAsync(null, "Some text to sign.");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateSignatureAsync_GivenNoTextToSign_ShouldThrowArgumentNullException()
        {
            var defaultSignerUt = new DefaultSigner();

            Func<Task> act = async () => await defaultSignerUt.GenerateSignatureAsync(DefaultRsaKeyXml, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GenerateSignatureAsync_GivenTextToSign_ShouldReturnValidSignature()
        {
            const string textToSign = "Some text to sign.";

            var defaultSignerUt = new DefaultSigner();

            var expectedSignature = GenerateSignature(textToSign);
            var actualSignature = await defaultSignerUt.GenerateSignatureAsync(DefaultRsaKeyXml, textToSign);

            actualSignature.Should().Be(expectedSignature);
        }

        [Fact]
        public async Task GenerateSignatureAsync_GivenDifferentTextToSign_ShouldReturnDifferentSignature()
        {
            const string textToSign = "Some text to sign.";
            const string otherTextToSign = "Some other text to sign.";

            var defaultSignerUt = new DefaultSigner();

            var textToSignSignature = await defaultSignerUt.GenerateSignatureAsync(DefaultRsaKeyXml, textToSign);
            var otherTextToSignSignature = await defaultSignerUt.GenerateSignatureAsync(DefaultRsaKeyXml, otherTextToSign);

            textToSignSignature.Should().NotBe(otherTextToSignSignature);
        }

        public string GenerateSignature(string textToSign)
        {
            using (var shaProvider = SHA512.Create())
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                rsaProvider.FromXmlString(DefaultRsaKeyXml);

                var toSignBytes = Encoding.UTF8.GetBytes(textToSign);
                var toSignHashBytes = shaProvider.ComputeHash(toSignBytes);
                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsaProvider);

                rsaFormatter.SetHashAlgorithm("SHA512");

                var signatureBytes = rsaFormatter.CreateSignature(toSignHashBytes);

                return Convert.ToBase64String(signatureBytes);
            }
        }
    }
}
