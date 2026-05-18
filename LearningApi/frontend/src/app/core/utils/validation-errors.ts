import { ValidationErrors } from '../models/api-response';

export function getFieldError(errors: ValidationErrors | null, field: string): string {
  return errors?.[field]?.[0] ?? '';
}