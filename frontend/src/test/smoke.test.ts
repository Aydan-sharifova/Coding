import { describe, expect, it } from "vitest";

describe("frontend test runtime", () => {
  it("executes TypeScript tests", () => {
    expect(2 + 2).toBe(4);
  });
});
