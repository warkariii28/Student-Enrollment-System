export const AppRoles = {
  Admin: 'Admin',
  User: 'User',
} as const;

export type AppRole = typeof AppRoles[keyof typeof AppRoles];