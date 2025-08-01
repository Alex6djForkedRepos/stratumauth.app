// Copyright (C) 2023 jmh
// SPDX-License-Identifier: GPL-3.0-only

using System.Linq;
using System.Threading.Tasks;
using Stratum.Core;
using Stratum.Core.Backup;
using Stratum.Core.Converter;
using Moq;
using Stratum.SQLCipher;
using Stratum.Test.Converter.Fixture;
using Xunit;

namespace Stratum.Test.Converter
{
    public class AuthenticatorPlusBackupConverterTest : IClassFixture<AuthenticatorPlusBackupFixture>
    {
        private readonly AuthenticatorPlusBackupFixture _authenticatorPlusBackupFixture;
        private readonly AuthenticatorPlusBackupConverter _authenticatorPlusBackupConverter;

        public AuthenticatorPlusBackupConverterTest(AuthenticatorPlusBackupFixture authenticatorPlusBackupFixture)
        {
            Provider.Init();
            
            _authenticatorPlusBackupFixture = authenticatorPlusBackupFixture;

            var iconResolver = new Mock<IIconResolver>();
            iconResolver.Setup(r => r.FindServiceKeyByName(It.IsAny<string>())).Returns("icon");

            _authenticatorPlusBackupConverter = new AuthenticatorPlusBackupConverter(iconResolver.Object);
        }

        [Fact]
        public async Task ConvertAsync_wrongPassword()
        {
            await Assert.ThrowsAsync<BackupPasswordException>(() =>
                _authenticatorPlusBackupConverter.ConvertAsync(_authenticatorPlusBackupFixture.Data, "test"));
        }

        [Fact]
        public async Task ConvertAsync_ok()
        {
            var result =
                await _authenticatorPlusBackupConverter.ConvertAsync(_authenticatorPlusBackupFixture.Data,
                    "testtesttest");

            Assert.Empty(result.Failures);

            Assert.Equal(8, result.Backup.Authenticators.Count());
            Assert.Single(result.Backup.Categories);
            Assert.Equal(2, result.Backup.AuthenticatorCategories.Count());
            Assert.Null(result.Backup.CustomIcons);
        }
    }
}