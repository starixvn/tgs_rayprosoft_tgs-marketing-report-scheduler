{{/*
Expand the name of the chart.
*/}}
{{- define "marketing-report-scheduler.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "marketing-report-scheduler.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "marketing-report-scheduler.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "marketing-report-scheduler.labels" -}}
helm.sh/chart: {{ include "marketing-report-scheduler.chart" . }}
{{ include "marketing-report-scheduler.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "marketing-report-scheduler.selectorLabels" -}}
app.kubernetes.io/name: {{ include "marketing-report-scheduler.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app: {{ include "marketing-report-scheduler.name" . }}
tgs.kind: report
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "marketing-report-scheduler.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "marketing-report-scheduler.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}
