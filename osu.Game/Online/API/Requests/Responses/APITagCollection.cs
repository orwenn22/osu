// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using Newtonsoft.Json;

namespace osu.Game.Online.API.Requests.Responses
{
    public class APITagCollection
    {
        [JsonProperty("tags")]
        public APITag[] Tags { get; set; } = Array.Empty<APITag>();
    }
}
