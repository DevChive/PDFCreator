using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public class InteractiveOutputFileMover : OutputFileMoverBase
    {
        private readonly IDispatcher _dispatcher;

        public InteractiveOutputFileMover(IDirectory directory, IFile file, IPathUtil pathUtil, IRetypeFileNameQuery retypeFileNameQuery, IDispatcher dispatcher, IDirectoryHelper directoryHelper)
        {
            _dispatcher = dispatcher;
            Directory = directory;
            DirectoryHelper = directoryHelper;
            File = file;
            PathUtil = pathUtil;
            RetypeFileNameQuery = retypeFileNameQuery;
        }

        protected override IDirectory Directory { get; }
        protected override IDirectoryHelper DirectoryHelper { get; }
        protected override IFile File { get; }
        protected override IPathUtil PathUtil { get; }
        private IRetypeFileNameQuery RetypeFileNameQuery { get; }

        protected override QueryResult<string> HandleInvalidRootedPath(string filename, OutputFormat outputFormat)
        {
            var result = _dispatcher.InvokeAsync(() => RetypeFileNameQuery.RetypeFileNameQuery(filename, outputFormat, RetypeReason.InvalidRootedPath));
            return result.Result;
        }

        protected override QueryResult<string> HandleFirstFileFailed(string filename, OutputFormat outputFormat)
        {
            var result = _dispatcher.InvokeAsync(() => RetypeFileNameQuery.RetypeFileNameQuery(filename, outputFormat, RetypeReason.CopyError));
            return result.Result;
        }

        protected override HandleCopyErrorResult QueryHandleCopyError(int fileNumber)
        {
            if (fileNumber == 1)
                return HandleCopyErrorResult.Requery;

            return HandleCopyErrorResult.EnsureUniqueFilename;
        }

        protected override bool ApplyUniqueFilename(Job job)
        {
            return false;
        }
    }
}
