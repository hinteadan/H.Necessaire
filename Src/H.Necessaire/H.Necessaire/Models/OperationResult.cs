using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public class OperationResult
    {
        public bool IsSuccessful { get; set; } = true;
        public string Reason { get; set; } = null;
        public string[] Comments { get; set; } = new string[0];

        public static OperationResult Win(string reason = null, params string[] comments)
        {
            return new OperationResult { IsSuccessful = true, Reason = reason, Comments = comments ?? new string[0] };
        }

        public static OperationResult Fail(string reason = null, params string[] comments)
        {
            return new OperationResult { IsSuccessful = false, Reason = reason, Comments = comments ?? new string[0] };
        }

        public static OperationResult Fail(Exception exception, string reason = null)
        {
            Exception[] all = exception.Flatten();

            string mainReason = !string.IsNullOrWhiteSpace(reason) ? reason : all.First().Message;

            return
                OperationResult.Fail(
                    mainReason,
                    all
                    .Skip(!string.IsNullOrWhiteSpace(reason) ? 0 : 1)
                    .Select(x => x.Message)
                    .Concat(
                        all
                        .Select((x, i) => $"Exception Details [{i}]:{Environment.NewLine}{x}")
                    )
                    .ToArray()
                );
        }

        public static OperationResult Fail(IEnumerable<string> errors)
        {
            if (!errors?.Any() ?? true)
                return Fail();

            return
                errors.Count() == 1
                ? Fail(errors.Single())
                : Fail("There are several issues with the current operation. Check the comments for details.", errors.ToArray());
        }

        public OperationResult WithComment(string comment)
        {
            Comments = (Comments ?? new string[0]).Concat(comment.AsArray()).ToArray();
            return this;
        }

        public OperationResult<T> WithPayload<T>(T payload)
        {
            return new OperationResult<T>(this, payload);
        }

        public OperationResult<T> WithoutPayload<T>()
        {
            return new OperationResult<T>(this);
        }

        public void ThrowOnFail()
        {
            if (IsSuccessful)
                return;

            throw new OperationResultException(this);
        }

        public string[] FlattenReasons()
        {
            return
                Reason
                .AsArray()
                .Concat(Comments ?? new string[0])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
    }
}
