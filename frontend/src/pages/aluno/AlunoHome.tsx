import {
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button,
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
      icon: <SchoolIcon sx={{ fontSize: 40 }} />,
      route: "/aluno/academico"
    },
    {
      title: "Boletim",
      description:
        "Visualize seu boletim completo e acompanhe seu desempenho geral.",
      icon: <AssignmentIcon sx={{ fontSize: 40 }} />,
      route: "/aluno/academico/boletim"
    },
    {
      title: "Eventos",
      description:
        "Consulte eventos acadêmicos e datas importantes.",
      icon: <EventIcon sx={{ fontSize: 40 }} />,
      route: "/aluno/eventos"
    },
    {
      title: "Matrícula",
      description:
        "Consulte a situação da sua matrícula e documentos enviados.",
      icon: <DescriptionIcon sx={{ fontSize: 40 }} />,
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

        {/* GRID DE CARDS */}
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
              sx={{
                p: 2,
                borderRadius: 3,
                border: "1px solid",
                borderColor: "divider",
                transition: "all 0.2s ease",
                "&:hover": {
                  boxShadow: 6,
                  transform: "translateY(-4px)"
                }
              }}
            >
              <CardContent>
                <Box display="flex" alignItems="center" gap={2} mb={2}>
                  {card.icon}
                  <Typography variant="h6" fontWeight={600}>
                    {card.title}
                  </Typography>
                </Box>

                <Typography variant="body2" color="text.secondary">
                  {card.description}
                </Typography>
              </CardContent>

              <CardActions sx={{ justifyContent: "flex-end", pr: 2 }}>
                <Button
                  variant="contained"
                  size="small"
                  onClick={() => navigate(card.route)}
                >
                  Acessar
                </Button>
              </CardActions>
            </Card>
          ))}
        </Box>

      </Box>
    </AppLayout>
  );
}