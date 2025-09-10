import { Request, Response } from 'express';

/**
 * Wraps an async route handler to provide uniform response payloads
 * and capture request latency.
 *
 * The returned handler automatically measures the time it takes to
 * execute the provided function and attaches that latency (in
 * milliseconds) to the response. On success, the payload is returned
 * under the `data` property; on failure, the error message is exposed
 * under the `error` property while `data` is set to `null`.
 */
export const handle = <T>(fn: (req: Request) => Promise<T>) => {
  return async (req: Request, res: Response): Promise<void> => {
    const start = process.hrtime.bigint();

    try {
      const data = await fn(req);
      const latency = Number(process.hrtime.bigint() - start) / 1_000_000;
      res.send({ data, latency, error: null });
    } catch (err: any) {
      const latency = Number(process.hrtime.bigint() - start) / 1_000_000;
      res.status(500).send({ data: null, latency, error: err?.message ?? 'Unexpected error' });
    }
  };
};

export default handle;
