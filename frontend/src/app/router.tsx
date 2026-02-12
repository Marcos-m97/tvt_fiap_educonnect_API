import { createBrowserRouter } from "react-router-dom";

import Login from "../pages/login/Login";
import AdminHome from "../pages/admin/AdminHome";
import AdminUsuarios from "../pages/admin/AdminUsuarios";
import ProfessorHome from "../pages/professor/ProfessorHome";
import AlunoHome from "../pages/aluno/AlunoHome";
import ForgotPassword from "../pages/auth/ForgotPassword";
import ResetPassword from "../pages/auth/ResetPassword";
import AdminUsuarioPerfil from "../pages/admin/AdminUsuarioPerfil";
import AdminUsuarioForm from "../pages/admin/AdminUsuarioForm";


import RoleGuard from "../guards/RoleGuard";

export const router = createBrowserRouter([
  // 🔓 ROTAS PÚBLICAS
  {
    path: "/login",
    element: <Login />,
  },
  {
    path: "/forgot-password",
    element: <ForgotPassword />,
  },
  {
    path: "/reset-password",
    element: <ResetPassword />,
  },

  // 🔐 ADMIN (roles 0 e 1)
  {
    path: "/admin",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminHome />
      </RoleGuard>
    ),
  },
  {
  path: "/admin/usuarios/:id",
  element: (
    <RoleGuard allowed={[0, 1]}>
      <AdminUsuarioPerfil />
    </RoleGuard>
  ),
},

  {
    path: "/admin/usuarios",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminUsuarios />
      </RoleGuard>
    ),
  },

  // 🔐 PROFESSOR (role 2)
  {
    path: "/professor",
    element: (
      <RoleGuard allowed={[2]}>
        <ProfessorHome />
      </RoleGuard>
    ),
  },

  // 🔐 ALUNO (role 3)
  {
    path: "/aluno",
    element: (
      <RoleGuard allowed={[3]}>
        <AlunoHome />
      </RoleGuard>
    ),
  },

  {
  path: "/admin/usuarios/novo",
  element: (
    <RoleGuard allowed={[0, 1]}>
      <AdminUsuarioForm />
    </RoleGuard>
  ),
},
{
  path: "/admin/usuarios/:id/editar",
  element: (
    <RoleGuard allowed={[0, 1]}>
      <AdminUsuarioForm />
    </RoleGuard>
  ),
},
  {
    path: "*",
    element: <Login />,
  },
]);
