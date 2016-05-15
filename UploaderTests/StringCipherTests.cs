using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using NUnit.Framework;
using Uploader;

[TestFixture]
public class StringCipherTests 
{

    [Test]
    public void EncryptDecrypt()
    {
        var passPhrase = "pawwsord";
        var cipher = new StringCipher(passPhrase);
        var plainText = "sekret";

        var encstring = cipher.EncryptString(plainText);
        Assert.AreEqual(plainText, cipher.DecryptString(encstring));
    }
}
