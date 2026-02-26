import { createBrowserRouter } from "react-router-dom";

import Login from "../pages/login/Login";
import ForgotPassword from "../pages/auth/ForgotPassword";
import ResetPassword from "../pages/auth/ResetPassword";

import AdminHome from "../pages/admin/AdminHome";

import AdminUsuarios from "../pages/admin/usuario/AdminUsuarios";
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

import ProfessorHome from "../pages/professor/ProfessorHome";

import ProfessorAcademico from "../pages/professor/academico/ProfessorAcademico";
import ProfessorTurmaDetalhe from "../pages/professor/academico/ProfessorTurmaDetalhe";
import ProfessorNovaAtividade from "../pages/professor/academico/ProfessorNovaAtividade";
import ProfessorGerenciarAtividade from "../pages/professor/academico/ProfessorGerenciarAtividade";
import ProfessorNovaAula from "../pages/professor/academico/ProfessorNovaAula";
import ProfessorGerenciarAula from "../pages/professor/academico/ProfessorGerenciarAula";
import ProfessorAlunoDetalhe from "../pages/professor/academico/ProfessorAlunoDetalhe";

import AlunoHome from "../pages/aluno/AlunoHome";

import EventosPage from "../pages/eventos/EventosPage";

import RoleGuard from "../guards/RoleGuard";

export const router = createBrowserRouter([

  // =========================
  // 🔓 ROTAS PÚBLICAS
  // =========================
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

  // =========================
  // 🔐 ADMIN (0 = SuperAdmin | 1 = Admin)
  // =========================
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

  // =========================
  // 🎓 ADMIN ACADÊMICO
  // =========================
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

  {
    path: "/admin/academico/alunos/:id",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminAlunoDetalhe />
      </RoleGuard>
    ),
  },

  // =========================
  // 📄 MATRÍCULAS
  // =========================
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

  // =========================
  // 📅 EVENTOS
  // =========================

  {
    path: "/admin/eventos",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <EventosPage />
      </RoleGuard>
    ),
  },

  {
    path: "/professor/eventos",
    element: (
      <RoleGuard allowed={[2]}>
        <EventosPage />
      </RoleGuard>
    ),
  },

  {
    path: "/aluno/eventos",
    element: (
      <RoleGuard allowed={[3]}>
        <EventosPage />
      </RoleGuard>
    ),
  },

  // =========================
  // 🔐 PROFESSOR
  // =========================

  {
    path: "/professor",
    element: (
      <RoleGuard allowed={[2]}>
        <ProfessorHome />
      </RoleGuard>
    ),
  },

  {
    path: "/professor/academico",
    element: (
      <RoleGuard allowed={[2]}>
        <ProfessorAcademico />
      </RoleGuard>
    ),
  },

  {
    path: "/professor/turma/:turmaDisciplinaId",
    element: (
      <RoleGuard allowed={[2]}>
        <ProfessorTurmaDetalhe />
      </RoleGuard>
    ),
  },
  {
  path: "/professor/academico/:turmaDisciplinaId/nova-atividade",
  element: (
    <RoleGuard allowed={[2]}>
      <ProfessorNovaAtividade />
    </RoleGuard>
  ),
},
{
  path: "/professor/academico/:turmaDisciplinaId/atividade/:atividadeId",
  element: (
    <RoleGuard allowed={[2]}>
      <ProfessorGerenciarAtividade />
    </RoleGuard>
  ),
},
{
  path: "/professor/academico/:turmaDisciplinaId/nova-aula",
  element: (
    <RoleGuard allowed={[2]}>
      <ProfessorNovaAula />
    </RoleGuard>
  ),
},
{
  path: "/professor/academico/:turmaDisciplinaId/aula/:aulaId",
  element: (
    <RoleGuard allowed={[2]}>
      <ProfessorGerenciarAula />
    </RoleGuard>
  ),
},
{
  path: "/professor/academico/:turmaDisciplinaId/aluno/:alunoId",
  element: (
    <RoleGuard allowed={[2]}>
      <ProfessorAlunoDetalhe />
    </RoleGuard>
  ),
},

  // =========================
  // 🔐 ALUNO
  // =========================

  {
    path: "/aluno",
    element: (
      <RoleGuard allowed={[3]}>
        <AlunoHome />
      </RoleGuard>
    ),
  },

  // =========================
  // Fallback
  // =========================

  {
    path: "*",
    element: <Login />,
  },
]);