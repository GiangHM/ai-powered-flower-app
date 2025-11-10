// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FlowerShop.Application.Dtos.Converters;
using System.Text.Json.Serialization;

namespace FlowerShop.Application.Dtos.ChatFeature;

[JsonConverter(typeof(JsonCamelCaseEnumConverter<AIChatRole>))]
public enum AIChatRole
{
    System,
    Assistant,
    User
}
