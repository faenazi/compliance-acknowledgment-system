namespace Eap.Domain.Forms;

/// <summary>
/// Single option for select/radio/multi-select fields (BR-075).
/// Stored as an embedded value object inside <see cref="FormField"/>.
/// </summary>
public sealed record FieldOption(string Value, string Label);
