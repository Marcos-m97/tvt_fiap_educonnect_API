import {
  Typography,
  Box,
  Card,
  CardContent,
  CircularProgress
} from "@mui/material";
import AppLayout from "../../components/layout/AppLayout";
import SchoolIcon from "@mui/icons-material/School";
import EventIcon from "@mui/icons-material/Event";
import AssignmentIcon from "@mui/icons-material/Assignment";
import DescriptionIcon from "@mui/icons-material/Description";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../services/api";

interface UsuarioResponse {
  usuario: {
    id: number;
    nome: string;
    email: string;
    tipo: number;
  };
}

interface ContextoResponse {
  tipoUsuario: string;
  alunoId: number;
  turmaId: number;
  turmaNome: string;
  cursoNome: string;
  disciplinas: {
    disciplinaId: number;
    nome: string;
  }[];
}

export default function AlunoHome() {
  const navigate = useNavigate();

  const [usuario, setUsuario] = useState<UsuarioResponse["usuario"] | null>(null);
  const [contexto, setContexto] = useState<ContextoResponse | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function carregarDados() {
      try {
        const me = await api.get<UsuarioResponse>("/account/me");
        const contextoRes = await api.get<ContextoResponse>("/account/me/contexto");

        setUsuario(me.data.usuario);
        setContexto(contextoRes.data);
      } catch (error) {
        console.error("Erro ao carregar dados do aluno:", error);
      } finally {
        setLoading(false);
      }
    }

    carregarDados();
  }, []);

  if (loading) {
    return (
      <AppLayout>
        <Box display="flex" justifyContent="center" mt={10}>
          <CircularProgress />
        </Box>
      </AppLayout>
    );
  }

  const cards = [
    {
      title: "Acadêmico",
      description:
        "Acesse suas disciplinas, aulas, atividades e visualize suas notas.",
      icon: <SchoolIcon sx={{ fontSize: 48 }} />,
      route: "/aluno/academico"
    },
    {
      title: "Boletim",
      description:
        "Visualize seu boletim completo e acompanhe seu desempenho geral.",
      icon: <AssignmentIcon sx={{ fontSize: 48 }} />,
      route: "/aluno/academico/boletim"
    },
    {
      title: "Eventos",
      description:
        "Consulte eventos acadêmicos e datas importantes.",
      icon: <EventIcon sx={{ fontSize: 48 }} />,
      route: "/aluno/eventos"
    },
    {
      title: "Matrícula",
      description:
        "Consulte a situação da sua matrícula e documentos enviados.",
      icon: <DescriptionIcon sx={{ fontSize: 48 }} />,
      route: "/aluno/matricula"
    }
  ];

  return (
    <AppLayout>
      <Box maxWidth="1000px" mx="auto">

        {/* HEADER */}
        <Box textAlign="center" mb={6}>
          <Typography
            variant="h3"
            sx={{
              fontWeight: 800,
              letterSpacing: "0.08em",
              mb: 2
            }}
          >
            Painel do{" "}
            <Box component="span" sx={{ color: "primary.main" }}>
              Aluno
            </Box>
          </Typography>

          <Typography variant="h6" color="text.secondary" sx={{ mb: 2 }}>
            Bem-vindo{usuario ? `, ${usuario.nome}` : ""}.
          </Typography>

          {contexto && (
            <Typography variant="body1" color="text.secondary">
              {contexto.cursoNome} • {contexto.turmaNome} •{" "}
              {contexto.disciplinas?.length || 0} disciplinas
            </Typography>
          )}
        </Box>

        {/* GRID */}
        <Box
          display="grid"
          gridTemplateColumns={{
            xs: "1fr",
            md: "1fr 1fr"
          }}
          gap={4}
        >
          {cards.map((card, index) => (
            <Card
              key={index}
              onClick={() => navigate(card.route)}
              sx={{
                p: 4,
                borderRadius: 4,
                border: "1px solid",
                borderColor: "divider",
                cursor: "pointer",
                transition: "all 0.25s ease",
                textAlign: "center",
                "&:hover": {
                  boxShadow: 8,
                  transform: "translateY(-6px)",
                  borderColor: "primary.main"
                }
              }}
            >
              <CardContent>

                <Box
                  display="flex"
                  justifyContent="center"
                  alignItems="center"
                  mb={2}
                  color="primary.main"
                >
                  {card.icon}
                </Box>

                <Typography
                  variant="h5"
                  fontWeight={700}
                  mb={1}
                >
                  {card.title}
                </Typography>

                <Typography
                  variant="body1"
                  color="text.secondary"
                  sx={{ maxWidth: "320px", mx: "auto" }}
                >
                  {card.description}
                </Typography>

              </CardContent>
            </Card>
          ))}
        </Box>

      </Box>
    </AppLayout>
  );
}