// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

namespace Sable.Samples.Core;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime DatePurchasedUtc { get; set; }
}
