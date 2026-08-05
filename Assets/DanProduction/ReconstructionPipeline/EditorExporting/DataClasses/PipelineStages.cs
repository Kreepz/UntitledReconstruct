public enum ExportStage
{
    Validating,
    Resolving,
    Compiling,
    Deploying
}

public enum ValidatingStage
{
    ValidatingMetadata,
    ValidatingHierarchy,
}

public enum ResolutionStage
{
    ResolvingDirectories,
    ResolvingVersion
}

public enum CompilationStage
{
    CompilingMetadata,
    CompilingThumbnail,
    CompilingLevel
}

public enum DeployStage
{
    DeployingMetadata,
    DeployingThumbnail,
    DeployingLevel
}