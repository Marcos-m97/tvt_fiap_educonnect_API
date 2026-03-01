import {
  Box,
  Typography,
  Button,
  Container,
  Stack,
  Card,
  CardContent
} from "@mui/material";
import SchoolIcon from "@mui/icons-material/School";
import MenuBookIcon from "@mui/icons-material/MenuBook";
import ComputerIcon from "@mui/icons-material/Computer";
import BusinessIcon from "@mui/icons-material/Business";
import CodeIcon from "@mui/icons-material/Code";
import { useNavigate } from "react-router-dom";

export default function LandingPage() {
  const navigate = useNavigate();

  const cursos = [
    {
      id: 1,
      nome: "Análise e Desenvolvimento de Sistemas",
      descricao:
        "Formação focada em desenvolvimento de software, arquitetura de sistemas e tecnologia aplicada.",
      icon: <ComputerIcon fontSize="large" />
    },
    {
      id: 2,
      nome: "Engenharia de Software",
      descricao:
        "Projetos, modelagem, qualidade e gestão de desenvolvimento de sistemas em larga escala.",
      icon: <CodeIcon fontSize="large" />
    },
    {
      id: 3,
      nome: "Sistemas de Informação",
      descricao:
        "Formação voltada à integração entre tecnologia e negócios, preparando profissionais para desenvolver soluções digitais e transformar dados em decisões estratégicas.",
      icon: <BusinessIcon fontSize="large" />
    },
    {
      id: 4,
      nome: "Ciência da Computação",
      descricao:
        "Base sólida em algoritmos, estruturas de dados, inteligência artificial e ciência de dados.",
      icon: <MenuBookIcon fontSize="large" />
    }
  ];

  return (
    <Box>

      {/* ================= HERO ================= */}
      <Box
        sx={{
          minHeight: "85vh",
          display: "flex",
          alignItems: "center",
          background: "linear-gradient(135deg, #1e3c72 0%, #2a5298 100%)",
          color: "white"
        }}
      >
        <Container maxWidth="md">
          <Box textAlign="center">
            <Box
              display="flex"
              justifyContent="center"
              alignItems="center"
              gap={2}
              mb={4}
            >
              <SchoolIcon sx={{ fontSize: 50 }} />
              <Typography variant="h3" fontWeight={800}>
                EduConnect
              </Typography>
            </Box>

            <Typography variant="h4" fontWeight={700} mb={2}>
              Conectando alunos, professores e conhecimento.
            </Typography>

            <Typography variant="h6" sx={{ opacity: 0.85 }} mb={6}>
              Uma plataforma moderna para gestão acadêmica, matrículas e
              acompanhamento de desempenho.
            </Typography>

            <Stack
              direction={{ xs: "column", sm: "row" }}
              spacing={3}
              justifyContent="center"
            >
              <Button
                variant="contained"
                size="large"
                sx={{ px: 5, py: 1.5, fontWeight: 600 }}
                onClick={() => navigate("/login")}
              >
                Entrar
              </Button>

              <Button
                variant="outlined"
                size="large"
                sx={{
                  px: 5,
                  py: 1.5,
                  fontWeight: 600,
                  borderColor: "white",
                  color: "white",
                  "&:hover": {
                    borderColor: "white",
                    backgroundColor: "rgba(255,255,255,0.1)"
                  }
                }}
                onClick={() => navigate("/register")}
              >
                Registrar-se
              </Button>
            </Stack>
          </Box>
        </Container>
      </Box>

      {/* ================= CURSOS ================= */}
      <Box py={10} bgcolor="#f5f7fb">
        <Container maxWidth="lg">
          <Box textAlign="center" mb={6}>
            <Typography variant="h4" fontWeight={800} mb={2}>
              Nossos Cursos
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Escolha a formação ideal para sua jornada acadêmica.
            </Typography>
          </Box>

          <Box
            display="grid"
            gridTemplateColumns={{
              xs: "1fr",
              sm: "1fr 1fr",
              md: "1fr 1fr"
            }}
            gap={4}
          >
            {cursos.map((curso) => (
              <Card
                key={curso.id}
                sx={{
                  p: 3,
                  height: "100%",
                  transition: "0.2s",
                  "&:hover": {
                    boxShadow: 6,
                    transform: "translateY(-6px)"
                  }
                }}
              >
                <CardContent>
                  <Box display="flex" alignItems="center" gap={2} mb={2}>
                    {curso.icon}
                    <Typography variant="h6" fontWeight={700}>
                      {curso.nome}
                    </Typography>
                  </Box>

                  <Typography
                    variant="body2"
                    color="text.secondary"
                    mb={3}
                  >
                    {curso.descricao}
                  </Typography>

                  <Button
                    variant="text"
                    onClick={() => navigate(`/curso/${curso.id}`)}
                  >
                    Ver detalhes
                  </Button>
                </CardContent>
              </Card>
            ))}
          </Box>
        </Container>
      </Box>

      {/* ================= DIFERENCIAIS ================= */}
      <Box py={10}>
        <Container maxWidth="lg">
          <Box textAlign="center" mb={6}>
            <Typography variant="h4" fontWeight={800} mb={2}>
              Por que escolher a EduConnect?
            </Typography>

            <Typography variant="body1" color="text.secondary">
              Tecnologia, inovação e acompanhamento acadêmico em um só lugar.
            </Typography>
          </Box>

          <Box
            display="grid"
            gridTemplateColumns={{
              xs: "1fr",
              md: "1fr 1fr 1fr"
            }}
            gap={4}
          >
            <Card sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight={700} mb={2}>
                Plataforma 100% Digital
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Acesso completo a aulas, materiais, atividades e boletim online.
              </Typography>
            </Card>

            <Card sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight={700} mb={2}>
                Professores Experientes
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Corpo docente com experiência acadêmica e de mercado.
              </Typography>
            </Card>

            <Card sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight={700} mb={2}>
                Acompanhamento de Desempenho
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Visualize notas, médias e evolução em tempo real.
              </Typography>
            </Card>
          </Box>
        </Container>
      </Box>

      {/* ================= NOTÍCIAS ================= */}
      <Box py={10} bgcolor="#f5f7fb">
        <Container maxWidth="lg">
          <Box textAlign="center" mb={6}>
            <Typography variant="h4" fontWeight={800} mb={2}>
              Últimas Notícias
            </Typography>

            <Typography variant="body1" color="text.secondary">
              Fique por dentro das novidades acadêmicas.
            </Typography>
          </Box>

          <Box
            display="grid"
            gridTemplateColumns={{
              xs: "1fr",
              md: "1fr 1fr 1fr"
            }}
            gap={4}
          >
            <Card sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight={700} mb={1}>
                Semana Acadêmica 2026
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Palestras com profissionais do mercado de tecnologia.
              </Typography>
            </Card>

            <Card sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight={700} mb={1}>
                Novo Laboratório de TI
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Infraestrutura modernizada para práticas de desenvolvimento.
              </Typography>
            </Card>

            <Card sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight={700} mb={1}>
                Programa de Iniciação Científica
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Projetos de pesquisa aplicada para alunos.
              </Typography>
            </Card>
          </Box>
        </Container>
      </Box>

      {/* ================= FOOTER ================= */}
      <Box py={4} bgcolor="#1e3c72" color="white" textAlign="center">
        <Typography variant="body2">
          © {new Date().getFullYear()} EduConnect — Todos os direitos reservados.
        </Typography>
      </Box>

    </Box>
  );
}