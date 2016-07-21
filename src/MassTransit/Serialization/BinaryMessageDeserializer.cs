// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
#if !NETCORE
namespace MassTransit.Serialization
{
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization.Formatters.Binary;
    using Newtonsoft.Json;


    public class BinaryMessageDeserializer :
        IMessageDeserializer
    {
        static readonly BinaryFormatter _formatter = new BinaryFormatter();
        readonly JsonSerializer _deserializer;
        readonly IPublishEndpointProvider _publishEndpoint;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public BinaryMessageDeserializer(JsonSerializer deserializer, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpoint)
        {
            _deserializer = deserializer;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public ContentType ContentType => BinaryMessageSerializer.BinaryContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            object obj;
            var headers = new Header[0];
            using (Stream body = receiveContext.GetBody())
            {
                obj = _formatter.Deserialize(body, x => headers = x);
            }

            return new StaticConsumeContext(_sendEndpointProvider, _publishEndpoint, receiveContext, obj, headers);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("binary");
            scope.Add("contentType", BinaryMessageSerializer.BinaryContentType.MediaType);
        }
    }
}
#endif