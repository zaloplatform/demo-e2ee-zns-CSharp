using libsignal;
using Newtonsoft.Json.Linq;
using TestE2EEZNS;
using libsignal.state;
using libsignal.state.impl;
using libsignal.ecc;
using System.Text;
using libsignal.protocol;
using libsignal.util;

string phoneNum = "";
string accessToken = "";

JObject body = new JObject();
body.Add("phone", phoneNum);

Dictionary<string, dynamic> param = new Dictionary<string, dynamic>();

Dictionary<string, string> header = new Dictionary<string, string>();
header.Add("access_token", accessToken);

string resultString = HttpHelper.sendHttpPostRequestWithBody("https://business.openapi.zalo.me/e2ee/key", param, body.ToString(), header);

JObject resultJson = null;
try
{
    resultJson = JObject.Parse(resultString);
} 
catch (Exception e)
{
    Console.WriteLine("Response is not json: " + resultString);
}

Console.WriteLine(resultJson);

JToken jtoken = resultJson.GetValue("data");
JObject data = jtoken.ToObject<JObject>();

if (data == null)
{
    Console.WriteLine("response data is null");
    return;
}

Console.WriteLine(data);

uint deviceId = data["device_id"].Value<uint>();

string pubKeyComplete = data["public_key_complete"].Value<string>();
uint pubKeyId = data["public_key_id"].Value<uint>();

string identityKeyComplete = data["identity_key_complete"].Value<string>();
uint identityId = data["identity_id"].Value<uint>();

string signature = data["signature"].Value<string>();

Console.WriteLine(deviceId);
Console.WriteLine(pubKeyComplete);
Console.WriteLine(pubKeyId);
Console.WriteLine(identityKeyComplete);
Console.WriteLine(identityId);
Console.WriteLine(signature);

Console.WriteLine(Convert.FromBase64String(signature).Length);

SignalProtocolAddress userAddress = new SignalProtocolAddress(phoneNum, deviceId);

SignalProtocolStore oaProtocolStore = new InMemorySignalProtocolStore(E2EEHelper.generateIdentityKeyPair(), E2EEHelper.generateRegistrationId());

byte[] pubKeyArray = Convert.FromBase64String(pubKeyComplete);
ECPublicKey publicKey = Curve.decodePoint(pubKeyArray, 0);

byte[] identityKeyArray = Convert.FromBase64String(identityKeyComplete);
IdentityKey identityKey = new IdentityKey(identityKeyArray, 0);

PreKeyBundle bobPreKeyBundle = new PreKeyBundle(identityId, deviceId,
                0, null,
                pubKeyId, publicKey,
                Convert.FromBase64String(signature), identityKey);

SessionBuilder aliceSessionBuilder = new SessionBuilder(oaProtocolStore, userAddress);

aliceSessionBuilder.process(bobPreKeyBundle);

SessionCipher aliceSessionCipher = new SessionCipher(oaProtocolStore, userAddress);

string message = "message e2ee from csharp 08/04/2022";
CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(message));

uint type = messageForBob.getType();

byte[] arrayFinalMessageType = { Convert.ToByte(type) };
byte[] arrayFinalMessageyData = messageForBob.serialize();
byte[] finalMessageByte = new byte[arrayFinalMessageType.Length + arrayFinalMessageyData.Length];
Array.Copy(arrayFinalMessageType, finalMessageByte, arrayFinalMessageType.Length);
Array.Copy(arrayFinalMessageyData, 0, finalMessageByte, arrayFinalMessageType.Length, arrayFinalMessageyData.Length);

string finalMessage = Convert.ToBase64String(finalMessageByte);

JObject templateData = new JObject();
templateData.Add("content", finalMessage);

JObject bodySendE2EE = new JObject();
bodySendE2EE.Add("phone", phoneNum);
bodySendE2EE.Add("template_id", "222461");
bodySendE2EE.Add("template_data", templateData);

Console.WriteLine(bodySendE2EE.ToString());