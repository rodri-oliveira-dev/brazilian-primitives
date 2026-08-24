namespace Brazilian.PrimitivesTypes;

internal readonly record struct RgStateRule(
    int CanonicalLength,
    RgMaskKind MaskKind,
    bool ValidateSaoPauloCheckDigit);
