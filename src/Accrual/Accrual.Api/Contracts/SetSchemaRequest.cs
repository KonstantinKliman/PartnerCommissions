using System.ComponentModel.DataAnnotations;
using Accrual.Domain;

namespace Accrual.Api.Contracts;

public sealed record SetSchemaRequest([Required] SchemaType? SchemaType);