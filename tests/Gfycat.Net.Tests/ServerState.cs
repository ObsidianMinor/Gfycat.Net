using System.Collections.Generic;
using System.IO;

namespace Gfycat.Net.Tests
{
    internal class ServerState
    {
        internal bool FollowUserSuccess { get; private set; }
        internal bool UnfollowUserSuccess { get; private set; }

        internal void ProcessState(string method, string endpoint)
        {
            switch(method)
            {
                case "PUT":
                    {
                        switch(endpoint)
                        {
                            case "me/follows/" + MockRestClient.ValidUser:
                                FollowUserSuccess = true;
                                break;
                        }
                    }
                    break;
                case "DELETE":
                    {
                        switch(endpoint)
                        {
                            case "me/follows/" + MockRestClient.ValidUser:
                                UnfollowUserSuccess = true;
                                break;
                        }
                    }
                    break;
            }
        }

        internal void ProcessState(string method, string endpoint, string json)
        {

        }

        internal void ProcessState(string method, string endpoint, Stream stream)
        {

        }

        internal void ProcessState(string method, string endpoint, IDictionary<string, object> multipart)
        {

        }
    }
}
