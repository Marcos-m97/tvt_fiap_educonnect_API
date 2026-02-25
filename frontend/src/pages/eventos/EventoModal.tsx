import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Box
} from "@mui/material";
import { useEffect, useState } from "react";
import { api } from "../../services/api";

interface Turma {
  id: number;
  nome: string;
}

interface TurmaDisciplina {
  id: number;
  disciplinaNome: string;
  turmaNome: string;
}

interface Props {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const TipoEventoOptions = [
  { value: 1, label: "Geral" },
  { value: 2, label: "Prova" },
  { value: 3, label: "Atividade" },
  { value: 4, label: "Aula Extra" },
  { value: 5, label: "Reunião" }
];

export default function EventoModal({
  open,
  onClose,
  onSuccess
}: Props) {
  const [titulo, setTitulo] = useState("");
  const [descricao, setDescricao] = useState("");
  const [inicio, setInicio] = useState("");
  const [fim, setFim] = useState("");
  const [tipo, setTipo] = useState(1);
  const [turmaId, setTurmaId] = useState<number | null>(null);
  const [turmaDisciplinaId, setTurmaDisciplinaId] = useState<number | null>(null);

  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [turmaDisciplinas, setTurmaDisciplinas] = useState<TurmaDisciplina[]>([]);

  useEffect(() => {
    api.get("/turma").then(res => setTurmas(res.data));
    api.get("/turmadisciplina").then(res => setTurmaDisciplinas(res.data));
  }, []);

  async function handleSubmit() {
    await api.post("/evento", {
      titulo,
      descricao,
      inicio,
      fim,
      tipo,
      turmaId,
      turmaDisciplinaId
    });

    onSuccess();
    onClose();
  }

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Novo Evento</DialogTitle>

      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>

          <TextField label="Título" value={titulo} onChange={e => setTitulo(e.target.value)} fullWidth />

          <TextField label="Descrição" value={descricao} onChange={e => setDescricao(e.target.value)} multiline rows={3} fullWidth />

          <TextField type="datetime-local" label="Início" InputLabelProps={{ shrink: true }} value={inicio} onChange={e => setInicio(e.target.value)} fullWidth />

          <TextField type="datetime-local" label="Fim" InputLabelProps={{ shrink: true }} value={fim} onChange={e => setFim(e.target.value)} fullWidth />

          <TextField select label="Tipo" value={tipo} onChange={e => setTipo(Number(e.target.value))} fullWidth>
            {TipoEventoOptions.map(opt => (
              <MenuItem key={opt.value} value={opt.value}>{opt.label}</MenuItem>
            ))}
          </TextField>

          <TextField
            select
            label="Turma (opcional)"
            value={turmaId ?? ""}
            onChange={(e) => {
              const value = e.target.value;
              setTurmaId(value ? Number(value) : null);
              setTurmaDisciplinaId(null);
            }}
            fullWidth
          >
            <MenuItem value="">Nenhuma</MenuItem>
            {turmas.map(t => (
              <MenuItem key={t.id} value={t.id}>{t.nome}</MenuItem>
            ))}
          </TextField>

          <TextField
            select
            label="Turma-Disciplina (opcional)"
            value={turmaDisciplinaId ?? ""}
            onChange={(e) => {
              const value = e.target.value;
              setTurmaDisciplinaId(value ? Number(value) : null);
            }}
            fullWidth
          >
            <MenuItem value="">Nenhuma</MenuItem>
            {turmaDisciplinas.map(td => (
              <MenuItem key={td.id} value={td.id}>
                {td.turmaNome} - {td.disciplinaNome}
              </MenuItem>
            ))}
          </TextField>

        </Box>
      </DialogContent>

      <DialogActions>
        <Button onClick={onClose}>Cancelar</Button>
        <Button variant="contained" onClick={handleSubmit}>Criar</Button>
      </DialogActions>
    </Dialog>
  );
}