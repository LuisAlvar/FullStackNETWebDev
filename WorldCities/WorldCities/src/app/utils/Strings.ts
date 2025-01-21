export function isNullOrEmpty(str: string | null | undefined): boolean {
  return (str ?? '').trim().length === 0;
}
