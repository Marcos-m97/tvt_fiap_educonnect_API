import { useEffect, useState } from "react";
import {
  Typography,
  Box,
  Button,
  CircularProgress
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import { useNavigate } from "react-router-dom";
import AppLayout from "../../components/layout/AppLayout";
import { useAuth } from "../../contexts/AuthContext";
import { api } from "../../services/api";

import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";

interface Evento {
  id: number;
  titulo: string;
  descricao?: string;
  inicio: string;
  fim?: string;
  tipo: number;
  turmaNome?: string;
  disciplinaNome?: string;
}

// 🔐 Tipos de usuário compatível com seu backend
const TipoUsuario = {
  SuperAdmin: 0,
  Admin: 1,
  Professor: 2,
  Aluno: 3,
} as const;

export default function EventosPage() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const [eventos, setEventos] = useState<Evento[]>([]);
  const [loading, setLoading] = useState(true);

  const isSuperAdmin = user?.tipo === TipoUsuario.SuperAdmin;
  const isAdmin = user?.tipo === TipoUsuario.Admin;
  const isProfessor = user?.tipo === TipoUsuario.Professor;
  const isAluno = user?.tipo === TipoUsuario.Aluno;

  const podeCriar = isSuperAdmin || isAdmin || isProfessor;

  useEffect(() => {
    async function carregarEventos() {
      try {
        const endpoint = isAluno ? "/evento/meus" : "/evento";
        const response = await api.get(endpoint);
        setEventos(response.data);
      } catch (error) {
        console.error("Erro ao carregar eventos:", error);
      } finally {
        setLoading(false);
      }
    }

    carregarEventos();
  }, [isAluno]);

  const eventosFormatados = eventos.map((e) => ({
    id: e.id.toString(), // obrigatório para FullCalendar
    title: `${e.titulo}${e.disciplinaNome ? ` - ${e.disciplinaNome}` : ""}`,
    start: e.inicio,
    end: e.fim,
  }));

  return (
    <AppLayout>
      <Box display="flex" justifyContent="space-between" alignItems="center">
        <Typography variant="h4">
          📅 Gestão de Eventos
        </Typography>

        {podeCriar && (
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate("novo")}
          >
            Novo Evento
          </Button>
        )}
      </Box>

      <Box mt={4}>
        {loading ? (
          <Box display="flex" justifyContent="center" mt={5}>
            <CircularProgress />
          </Box>
        ) : (
          <FullCalendar
            plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
            initialView="dayGridMonth"
            height="auto"
            locale="pt-br"
            headerToolbar={{
              left: "prev,next today",
              center: "title",
              right: "dayGridMonth,timeGridWeek,timeGridDay",
            }}
            events={eventosFormatados}
            eventClick={(info) => {
              if (podeCriar) {
                navigate(`${info.event.id}`);
              }
            }}
          />
        )}
      </Box>
    </AppLayout>
  );
}