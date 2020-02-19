// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Models.Interfaces;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Services
{
    public class TestExtensionRsaKeyProvider : IExtensionRsaKeyProvider
    {
        public Task<string> GetExtensionRsaKeyXmlAsync(Extension extension)
        {
            // For default/testing purposes only!
            // In production, each extension should own its own key pair.

            return Task.FromResult(
                @"<RSAKeyValue>
                    <Modulus>r/WV+SzUls2Imn6JLS1h/hZpz9+Ei7bWcWXI136gWeeUjedYAea/dncsgA9BA1Zga9z4B/x9Q/AsqY2ZqtA9ovyTCObyokbOnHflazPYo6oiwwGPZHD9DB92HncYNdGuD4oIGXYbymK+7QGR2odq5wgPjn2sp+BR1idpNa5Yci0=</Modulus>
                    <Exponent>AQAB</Exponent>
                    <P>2R4dd6E4aGg6NQnnwcKA0fPBoKvbBESfTRl9w8KKf0tzyJPd3Fml0KNwKnC2kD2S4/UmSJWrlsNN0pyNM5Xivw==</P>
                    <Q>z3iLKsaeYScLJidRALBcHPI/2d5Lf7YzKdBwFk6H+TjDdad5acUAwfvUpEgfaPVwj2LAJe+hIlj/nZcoFvTiEw==</Q>
                    <DP>sDQLkCHsSHeF/bNrPcmfiER9+OpgFeenLQgqE+xXQBW5AqhWXtT2iAuMJTVSRF9kTdKP3kpxdipMh0d8JMhYuw==</DP>
                    <DQ>zltGiLBvXZB5DWTzs6y2oe/txGTPUWLG9uMkaXuI0UE9YGvIXoSdIVS7Bv/ZhgsB/wwotFLpQYGmFXgAXv0JUw==</DQ>
                    <InverseQ>ZhgSXqzsewf/S+oiOGYkZUlbchrqAMzWRbGHQYRua1biA9gfMqD5klQxriPJWyD0T/3fpNFa9pwQshx7rb2TRQ==</InverseQ>
                    <D>dYqlg41Z+d8UyLeMZTxywGwyhOU4QqiBucCXmLSC42vahayr0qFU9wsHFAickmEqCgPTQGp/N1oCS8VGiJ+Q++sBpUFEQkjJfiSzjxrHNSkwncrXSpW/rcqU04iiEw/l7xI7luC9xiWqcA5/gBaM6R8YTFDLJo4bqddRCxIgfBU=</D>
                </RSAKeyValue>");
        }
    }
}