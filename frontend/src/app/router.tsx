import { createBrowserRouter } from "react-router-dom";

import Login from "../pages/login/Login";
import AdminHome from "../pages/admin/AdminHome";
import AdminUsuarios from "../pages/admin/usuario/AdminUsuarios";
import ProfessorHome from "../pages/professor/ProfessorHome";
import AlunoHome from "../pages/aluno/AlunoHome";
import ForgotPassword from "../pages/auth/ForgotPassword";
import ResetPassword from "../pages/auth/ResetPassword";

import AdminUsuarioPerfil from "../pages/admin/usuario/AdminUsuarioPerfil";
import AdminUsuarioForm from "../pages/admin/usuario/AdminUsuarioForm";
import AdminUsuarioEditar from "../pages/admin/usuario/AdminUsuarioEditar";

import AdminCursos from "../pages/admin/academico/AdminCursos";
import AdminCursoForm from "../pages/admin/academico/AdminCursoForm";
import AdminCursoDetalhe from "../pages/admin/academico/AdminCursoDetalhe";
import AdminCursoEditar from "../pages/admin/academico/AdminCursoEditar";

import AdminTurmaDetalhe from "../pages/admin/academico/AdminTurmaDetalhe";
import AdminTurmaForm from "../pages/admin/academico/AdminTurmaForm";
import AdminTurmaEditar from "../pages/admin/academico/AdminTurmaEditar";

import AdminDisciplinaForm from "../pages/admin/academico/AdminDisciplinaForm";
import AdminDisciplinaEditar from "../pages/admin/academico/AdminDisciplinaEditar";

import AdminAlunoDetalhe from "../pages/admin/academico/AdminAlunoDetalhe";

import AdminMatriculas from "../pages/admin/matriculas/AdminMatriculas";
import AdminMatriculaDetalhe from "../pages/admin/matriculas/AdminMatriculaDetalhe";

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
    path: "/admin/usuarios",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminUsuarios />
      </RoleGuard>
    ),
  },

  // 🔹 Usuários (rotas mais específicas primeiro)
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
        <AdminUsuarioEditar />
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

  // 🔹 Acadêmico - Cursos
  {
    path: "/admin/academico/cursos",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminCursos />
      </RoleGuard>
    ),
  },
  {
    path: "/admin/academico/cursos/novo",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminCursoForm />
      </RoleGuard>
    ),
  },
  {
    path: "/admin/academico/cursos/:id/editar",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminCursoEditar />
      </RoleGuard>
    ),
  },
  {
    path: "/admin/academico/cursos/:id",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminCursoDetalhe />
      </RoleGuard>
    ),
  },

  // 🔹 Acadêmico - Turmas
  {
    path: "/admin/academico/turmas/novo",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminTurmaForm />
      </RoleGuard>
    ),
  },
  {
    path: "/admin/academico/turmas/:id/editar",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminTurmaEditar />
      </RoleGuard>
    ),
  },
  {
    path: "/admin/academico/turmas/:id",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminTurmaDetalhe />
      </RoleGuard>
    ),
  },

  // 🔹 Acadêmico - Disciplinas
  {
    path: "/admin/academico/disciplinas/novo",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminDisciplinaForm />
      </RoleGuard>
    ),
  },
  {
    path: "/admin/academico/disciplinas/:id",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminDisciplinaEditar />
      </RoleGuard>
    ),
  },

  // 🔹 Acadêmico - Aluno Detalhe
  {
    path: "/admin/academico/alunos/:id",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminAlunoDetalhe />
      </RoleGuard>
    ),
  },
    // 🔹 Matrículas
  {
    path: "/admin/matriculas",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminMatriculas />
      </RoleGuard>
    ),
  },
  {
  path: "/admin/matriculas/:id",
  element: (
    <RoleGuard allowed={[0, 1]}>
      <AdminMatriculaDetalhe />
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

  // fallback
  {
    path: "*",
    element: <Login />,
  },
]);