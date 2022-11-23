#include "material.h"
#include "texture.h"
#include "application.h"
#include "extra/hdre.h"

StandardMaterial::StandardMaterial()
{
	color = vec4(1.f, 1.f, 1.f, 1.f);
	shader = Shader::Get("data/shaders/basic.vs", "data/shaders/flat.fs");
}

StandardMaterial::~StandardMaterial()
{

}

void StandardMaterial::setUniforms(Camera* camera, Matrix44 model)
{

	Matrix44 auxModel = model;
	auxModel.inverse();
	Vector3 localEye =  auxModel * camera->eye;

	//upload node uniforms
	shader->setUniform("u_viewprojection", camera->viewprojection_matrix);
	shader->setUniform("u_camera_pos", localEye);
	shader->setUniform("u_model", model);
	shader->setUniform("u_time", Application::instance->time);
	shader->setUniform("u_color", color);
	shader->setUniform("u_brightness", brightness);
	shader->setUniform("u_steplength", stepLength);

	if (texture)
		shader->setUniform("u_texture", texture);
}

void StandardMaterial::render(Mesh* mesh, Matrix44 model, Camera* camera)
{
	if (mesh && shader)
	{
		//enable shader
		shader->enable();

		//upload uniforms
		setUniforms(camera, model);

		//do the draw call
		mesh->render(GL_TRIANGLES);

		//disable shader
		shader->disable();
	}
}

void StandardMaterial::renderInMenu()
{
	ImGui::ColorEdit3("Color", (float*)&color); // Edit 3 floats representing a color
	ImGui::SliderFloat("Brightness", (float*)&brightness, 0, 10);
	ImGui::SliderFloat("StepLength", (float*)&stepLength, 0.01, 0.1);
}

WireframeMaterial::WireframeMaterial()
{
	color = vec4(1.f, 1.f, 1.f, 1.f);
	shader = Shader::Get("data/shaders/basic.vs", "data/shaders/flat.fs");
}

WireframeMaterial::~WireframeMaterial()
{

}

void WireframeMaterial::render(Mesh* mesh, Matrix44 model, Camera* camera)
{
	if (shader && mesh)
	{
		glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);

		//enable shader
		shader->enable();

		//upload material specific uniforms
		setUniforms(camera, model);

		//do the draw call
		mesh->render(GL_TRIANGLES);

		glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
	}
}


VolumeMaterial::VolumeMaterial()
{
	color = vec4(1.f, 1.f, 1.f, 1.f);
	brightness = 5;
	stepLength = 0.03;
	shader = Shader::Get("data/shaders/basic.vs", "data/shaders/volume.fs");
}

VolumeMaterial::~VolumeMaterial()
{

}

void VolumeMaterial::renderInMenu()
{
	ImGui::ColorEdit3("Color", (float*)&color); // Edit 3 floats representing a color
	ImGui::SliderFloat("Brightness", (float*)&brightness, 0, 10);
	ImGui::SliderFloat("StepLength", (float*)&stepLength, 0.01, 0.1);
	ImGui::Checkbox("use Jittering", (bool*)&useJittering);
	ImGui::Checkbox("use TF", (bool*)&useTransfer);
	ImGui::Checkbox("use Clipping", (bool*)&useClipping);
}

void VolumeMaterial::render(Mesh* mesh, Matrix44 model, Camera* camera)
{
	if (shader && mesh)
	{
		glEnable(GL_CULL_FACE);
		glCullFace(GL_BACK);
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);

		//enable shader
		shader->enable();

		//upload material specific uniforms
		this->setUniforms(camera, model);

		//do the draw call
		mesh->render(GL_TRIANGLES);

		glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
	}
}

void VolumeMaterial::setUniforms(Camera* camera, Matrix44 model)
{
	Matrix44 auxModel = model;
	auxModel.inverse();
	Vector3 localEye = auxModel * camera->eye;

	//upload node uniforms
	shader->setUniform("u_viewprojection", camera->viewprojection_matrix);
	shader->setUniform("u_camera_pos", localEye);
	shader->setUniform("u_model", model);
	shader->setUniform("u_time", Application::instance->time);
	shader->setUniform("u_color", color);
	shader->setUniform("u_brightness", brightness);
	shader->setUniform("u_steplength", stepLength);
	shader->setUniform("u_noise", noise);
	shader->setUniform("u_noisewidth", noise->width);
	shader->setUniform("u_usetransfer", useTransfer);
	shader->setUniform("u_usejittering", useJittering);
	shader->setUniform("u_useclipping", useClipping);
	shader->setUniform("u_tf", tf);
	shader->setUniform("u_plane", plane);

	if (texture)
		shader->setUniform("u_texture", texture);
}