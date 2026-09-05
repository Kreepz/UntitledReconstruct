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
    ResolvingVersion,
    ResolvingImageCompiler,
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