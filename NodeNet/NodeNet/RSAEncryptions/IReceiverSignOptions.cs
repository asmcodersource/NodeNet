using NodeNet.NodeNet.SignOptions;


/*
 * I have a complete mess here with classes aimed at implementing digital signatures, 
 * somehow I brought two absolutely Identical sets of classes into the project, 
 * and mixed them using them in different parts of the program. 
 * Now it needs to be cleaned, I hope I get around to it.
 * TODO: clean this shit...
 */
namespace NodeNet.NodeNet.RSAEncryptions
{
    public interface IReceiverSignOptions: SignOptions.IReceiverSignOptions {}
}
