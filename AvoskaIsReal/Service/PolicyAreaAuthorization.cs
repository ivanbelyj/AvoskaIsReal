using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace AvoskaIsReal.Service
{
    public class PolicyAreaAuthorization : IControllerModelConvention
    {
        private readonly string _policy;
        private readonly string _area;

        public PolicyAreaAuthorization(string policy, string area)
        {
            _policy = policy;
            _area = area;
        }

        public void Apply(ControllerModel controller)
        {
            // Если у контроллера есть area
            bool hasArea = controller.Attributes.Any(r => r is AreaAttribute &&
                ((AreaAttribute)r).RouteValue.Equals(_area,
                StringComparison.OrdinalIgnoreCase));
            bool hasArea1 = controller.RouteValues.Any(r => r.Key.Equals("area")
                && r.Value != null && r.Value.Equals(_area));
            if (hasArea || hasArea1) {
                controller.Filters.Add(new AuthorizeFilter(_policy));
            }
        }
    }
}
